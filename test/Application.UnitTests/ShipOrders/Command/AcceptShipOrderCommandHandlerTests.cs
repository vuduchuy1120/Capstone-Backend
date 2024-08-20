using Application.Abstractions.Data;
using Application.UserCases.Commands.ShipOrders.AcceptShipOrder;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.AcceptShipOrder;
using Contract.Services.ShipOrder.Share;
using Contract.Services.ShipOrder.Create;
using Domain.Entities;
using Domain.Exceptions.ShipOrder;
using Moq;
using Application.Utils;

public class AcceptShipOrderCommandHandlerTests
{
    private readonly Mock<IShipOrderRepository> _mockShipOrderRepository;
    private readonly Mock<IOrderDetailRepository> _mockOrderDetailRepository;
    private readonly Mock<IProductPhaseRepository> _mockProductPhaseRepository;
    private readonly Mock<ISetRepository> _mockSetRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly AcceptShipOrderCommandHandler _handler;
    private readonly Mock<ShipOrderUtil> _shipOrderUtilMock; 

    public AcceptShipOrderCommandHandlerTests()
    {
        _mockShipOrderRepository = new Mock<IShipOrderRepository>();
        _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();
        _mockProductPhaseRepository = new Mock<IProductPhaseRepository>();
        _mockSetRepository = new Mock<ISetRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _shipOrderUtilMock = new();
        _handler = new AcceptShipOrderCommandHandler(
            _mockShipOrderRepository.Object,
            _mockOrderDetailRepository.Object,
            _mockProductPhaseRepository.Object,
            _mockSetRepository.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowShipOrderNotFoundException_WhenShipOrderNotFound()
    {
        // Arrange
        var request = new AcceptShipOrderCommand(Guid.NewGuid(), "user123");
        _mockShipOrderRepository.Setup(repo => repo.GetByIdAndStatusIsNotDoneAsync(request.shipOrderId))
            .ReturnsAsync((ShipOrder)null);

        // Act & Assert
        await Assert.ThrowsAsync<ShipOrderNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowShipOrderBadRequestException_WhenShipOrderStatusIsInvalid()
    {
        // Arrange
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
        var request = new AcceptShipOrderCommand (shipOrder.Id, "user123");

        _mockShipOrderRepository.Setup(repo => repo.GetByIdAndStatusIsNotDoneAsync(shipOrder.Id))
            .ReturnsAsync(shipOrder);

        // Act & Assert
        await Assert.ThrowsAsync<ShipOrderDetailNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowShipOrderNotFoundException_WhenShipOrderAlreadyAccepted()
    {
        // Arrange
        var request = new AcceptShipOrderCommand(Guid.NewGuid(), "user123");
        _mockShipOrderRepository.Setup(repo => repo.GetByIdAndStatusIsNotDoneAsync(request.shipOrderId))
            .ReturnsAsync((ShipOrder)null);

        // Act & Assert
        await Assert.ThrowsAsync<ShipOrderNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldUpdateShipOrder_WhenRequestIsValidAndStatusIsShipped()
    {
        // Arrange
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
        var request = new AcceptShipOrderCommand(shipOrder.Id, "user123");
        shipOrder.UpdateStatus(Status.SHIPPED, "Dihson103");

        _mockShipOrderRepository.Setup(repo => repo.GetByIdAndStatusIsNotDoneAsync(shipOrder.Id))
            .ReturnsAsync(shipOrder);

        await Assert.ThrowsAsync<ShipOrderDetailNotFoundException>(() => _handler.Handle(request, CancellationToken.None));

    }

    [Fact]
    public async Task Handle_ShouldUpdateShipOrder_WhenRequestIsValidAndStatusIsCancel()
    {
        // Arrange
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
        var request = new AcceptShipOrderCommand(shipOrder.Id, "user123");
        shipOrder.UpdateStatus(Status.CANCEL, "Dihson103");

        _mockShipOrderRepository.Setup(repo => repo.GetByIdAndStatusIsNotDoneAsync(shipOrder.Id))
            .ReturnsAsync(shipOrder);

        await Assert.ThrowsAsync<ShipOrderDetailNotFoundException>(() => _handler.Handle(request, CancellationToken.None));

    }


}
