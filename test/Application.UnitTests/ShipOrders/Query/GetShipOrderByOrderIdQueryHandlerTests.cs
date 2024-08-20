

using Application.Abstractions.Data;
using Application.UserCases.Queries.ShipOrders.GetShipOrderByOrderId;
using AutoMapper;
using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.GetShipOrderByOrderId;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Domain.Exceptions.ShipOrder;
using FluentAssertions;
using Moq;

namespace Application.UnitTests.ShipOrders.Query;

public class GetShipOrderByOrderIdQueryHandlerTests
{
    private readonly Mock<IShipOrderRepository> _shipOrderRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetShipOrderByOrderIdQueryHandler _handler;

    public GetShipOrderByOrderIdQueryHandlerTests()
    {
        _shipOrderRepositoryMock = new Mock<IShipOrderRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetShipOrderByOrderIdQueryHandler(_shipOrderRepositoryMock.Object, _mapperMock.Object);
    }

    
    public async Task Handle_WhenShipOrderExists_ShouldReturnSuccessWithShipOrderResponses()
    {
        // Arrange
        var query = new GetShipOrderByOrderIdQuery(Guid.NewGuid());
        var shipOrderDetailRequests = new List<ShipOrderDetailRequest>
        {
            new ShipOrderDetailRequest(Guid.NewGuid(), 10, ItemKind.PRODUCT),
            new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.SET)
        };

        // Create the CreateShipOrderRequest object with necessary details
        var request = new CreateShipOrderRequest(
            ShipperId: "some-shipper-id",
            KindOfShipOrder: DeliveryMethod.SHIP_ORDER,
            OrderId: Guid.NewGuid(), // replace with actual OrderId
            ShipDate: DateTime.UtcNow,
            ShipOrderDetailRequests: shipOrderDetailRequests
        );

        // Use the static Create method to instantiate the ShipOrder
        var shipOrder = ShipOrder.Create("createdByUser", request);
        var shipOrderList = new List<ShipOrder> { shipOrder };

        _shipOrderRepositoryMock
            .Setup(repo => repo.GetByOrderIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(shipOrderList);


        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _shipOrderRepositoryMock.Verify(repo => repo.GetByOrderIdAsync(query.id), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenShipOrderIsNull_ShouldReturnSuccessWithNull()
    {
        // Arrange
        var query = new GetShipOrderByOrderIdQuery( Guid.NewGuid() );
        _shipOrderRepositoryMock
            .Setup(repo => repo.GetByOrderIdAsync(query.id))
            .ReturnsAsync((List<ShipOrder>)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WhenShipOrderDetailIsNull_ShouldThrowShipOrderDetailNotFoundException()
    {
        // Arrange
        var query = new GetShipOrderByOrderIdQuery(Guid.NewGuid());

        // Create the CreateShipOrderRequest object with necessary details
        var request = new CreateShipOrderRequest(
            ShipperId: "some-shipper-id",
            KindOfShipOrder: DeliveryMethod.SHIP_ORDER,
            OrderId: Guid.NewGuid(), // replace with actual OrderId
            ShipDate: DateTime.UtcNow,
            ShipOrderDetailRequests: null
        );

        // Use the static Create method to instantiate the ShipOrder
        var shipOrder = ShipOrder.Create("createdByUser", request);
        var shipOrderList = new List<ShipOrder> { shipOrder };

        _shipOrderRepositoryMock
            .Setup(repo => repo.GetByOrderIdAsync(query.id))
            .ReturnsAsync(shipOrderList);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ShipOrderDetailNotFoundException>();
        _shipOrderRepositoryMock.Verify(repo => repo.GetByOrderIdAsync(query.id), Times.Once);
    }

    
}
