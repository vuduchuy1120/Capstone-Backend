using Application.Abstractions.Data;
using Application.UserCases.Commands.ShipOrders.ChangeStatus;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.ChangeStatus;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Domain.Exceptions.Shipments;
using Domain.Exceptions.ShipOrder;
using Moq;

namespace Application.UnitTests.ShipOrders.Command;

public class ChangeShipOrderStatusCommandHandlerTests
{
    private readonly Mock<IShipOrderRepository> _mockShipOrderRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ChangeShipOrderStatusCommandHandler _handler;

    public ChangeShipOrderStatusCommandHandlerTests()
    {
        _mockShipOrderRepository = new Mock<IShipOrderRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new ChangeShipOrderStatusCommandHandler(_mockShipOrderRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowShipOrderIdConflictException_WhenIdsDoNotMatch()
    {
        var ChangeShipOrderStatusRequest = new ChangeShipOrderStatusRequest(Guid.NewGuid(), Status.CANCEL);
        // Arrange
        var request = new ChangeShipOrderStatusCommand("UpdateBy", Guid.NewGuid(), ChangeShipOrderStatusRequest);

        // Act & Assert
        await Assert.ThrowsAsync<ShipOrderIdConflictException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowShipmentBadRequestException_WhenStatusIsInvalid()
    {
        // Arrange
        var shipOrderId = Guid.NewGuid();
        var ChangeShipOrderStatusRequest = new ChangeShipOrderStatusRequest(shipOrderId, (Status)99);
        // Arrange
        var request = new ChangeShipOrderStatusCommand("UpdateBy", shipOrderId, ChangeShipOrderStatusRequest);
       

        // Act & Assert
        await Assert.ThrowsAsync<ShipmentBadRequestException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowShipOrderNotFoundException_WhenShipOrderNotFound()
    {
        // Arrange
        var shipOrderId = Guid.NewGuid();
        var ChangeShipOrderStatusRequest = new ChangeShipOrderStatusRequest(shipOrderId, Status.SHIPPING);
        // Arrange
        var request = new ChangeShipOrderStatusCommand("UpdateBy", shipOrderId, ChangeShipOrderStatusRequest);

        _mockShipOrderRepository.Setup(repo => repo.GetByIdAndStatusIsNotDoneAsync(shipOrderId))
            .ReturnsAsync((ShipOrder)null);

        // Act & Assert
        await Assert.ThrowsAsync<ShipOrderNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldUpdateShipOrderStatus_WhenRequestIsValid()
    {
        // Arrange
        var shipOrderId = Guid.NewGuid();
        var shipOrderDetailRequests = new List<ShipOrderDetailRequest>
        {
            new ShipOrderDetailRequest(Guid.NewGuid(), 10, ItemKind.PRODUCT),
            new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.SET)
        };

        var CreateShipOrderRequest = new CreateShipOrderRequest(
            ShipperId: "some-shipper-id",
            KindOfShipOrder: DeliveryMethod.SHIP_ORDER,
            OrderId: Guid.NewGuid(), // replace with actual OrderId
            ShipDate: DateTime.UtcNow,
            ShipOrderDetailRequests: shipOrderDetailRequests
        );

        var shipOrder = ShipOrder.Create("createdByUser", CreateShipOrderRequest);

        var ChangeShipOrderStatusRequest = new ChangeShipOrderStatusRequest(shipOrderId, Status.SHIPPED);
        // Arrange
        var request = new ChangeShipOrderStatusCommand("UpdateBy", shipOrderId, ChangeShipOrderStatusRequest);

        _mockShipOrderRepository.Setup(repo => repo.GetByIdAndStatusIsNotDoneAsync(shipOrderId))
            .ReturnsAsync(shipOrder);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockShipOrderRepository.Verify(repo => repo.Update(shipOrder), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(result);
        Assert.IsType<Result.Success>(result);
        Assert.Equal(Status.SHIPPED, shipOrder.Status);
        Assert.Equal("UpdateBy", shipOrder.UpdatedBy);
    }
}

