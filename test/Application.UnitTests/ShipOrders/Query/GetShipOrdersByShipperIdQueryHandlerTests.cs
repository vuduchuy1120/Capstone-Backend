

using Application.Abstractions.Data;
using Application.UserCases.Queries.ShipOrders.GetShipOrdersByShipper;
using AutoMapper;
using Contract.Services.Company.ShareDtos;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.GetShipOrdersOfShipper;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.ShipOrders.Query;

public class GetShipOrdersByShipperIdQueryHandlerTests
{
    private readonly Mock<IShipOrderRepository> _mockShipOrderRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetShipOrdersByShipperIdQueryHandler _handler;

    public GetShipOrdersByShipperIdQueryHandlerTests()
    {
        _mockShipOrderRepository = new Mock<IShipOrderRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetShipOrdersByShipperIdQueryHandler(_mockShipOrderRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoData_WhenShipperNotFound()
    {
        var SearchShipOrderOption = new SearchShipOrderOption(DateTime.Now, Status.SHIPPED);
        var request = new GetShipOrdersByShipperIdQuery("000000", SearchShipOrderOption);

        _mockShipOrderRepository.Setup(repo => repo.SearchShipOrderByShipperAsync(request)).ReturnsAsync((null, 0));

        var result = await _handler.Handle(request, default);

        Assert.NotNull(result);
        Assert.Equal(0, result.data.Data.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoData_WhenShipperExistButNoData()
    {
        var SearchShipOrderOption = new SearchShipOrderOption(DateTime.Now, Status.SHIPPED);
        var request = new GetShipOrdersByShipperIdQuery("existed shipper", SearchShipOrderOption);

        _mockShipOrderRepository.Setup(repo => repo.SearchShipOrderByShipperAsync(request)).ReturnsAsync((null, 0));

        var result = await _handler.Handle(request, default);

        Assert.NotNull(result);
        Assert.Equal(0, result.data.Data.Count);
    }

    public async Task Handle_ShouldReturnNoData_WhenShipperExistAndHasData()
    {
        var SearchShipOrderOption = new SearchShipOrderOption(DateTime.Now, Status.SHIPPED);
        var request = new GetShipOrdersByShipperIdQuery("existed shipper", SearchShipOrderOption);

        var shipOrderDetailRequests = new List<ShipOrderDetailRequest>
        {
            new ShipOrderDetailRequest(Guid.NewGuid(), 10, ItemKind.PRODUCT),
            new ShipOrderDetailRequest(Guid.NewGuid(), 5, ItemKind.SET)
        };

        // Create the CreateShipOrderRequest object with necessary details
        var CreateShipOrderRequest = new CreateShipOrderRequest(
            ShipperId: "some-shipper-id",
            KindOfShipOrder: DeliveryMethod.SHIP_ORDER,
            OrderId: Guid.NewGuid(), // replace with actual OrderId
            ShipDate: DateTime.UtcNow,
            ShipOrderDetailRequests: shipOrderDetailRequests
        );

        // Use the static Create method to instantiate the ShipOrder
        var shipOrder = ShipOrder.Create("createdByUser", CreateShipOrderRequest);
        var shipOrderList = new List<ShipOrder> { shipOrder };

        _mockShipOrderRepository.Setup(repo => repo.SearchShipOrderByShipperAsync(request)).ReturnsAsync((shipOrderList, 1));

        var result = await _handler.Handle(request, default);

        Assert.NotNull(result);
        Assert.NotEqual(0, result.data.Data.Count);
    }
}