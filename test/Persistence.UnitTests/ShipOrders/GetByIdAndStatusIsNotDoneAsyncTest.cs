using Application.Abstractions.Data;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ShipOrders;

public class GetByIdAndStatusIsNotDoneAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IShipOrderRepository _shipOrderRepository;
    public GetByIdAndStatusIsNotDoneAsyncTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _shipOrderRepository = new ShipOrderRepository(_context);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var shipOrderRequests = new List<CreateShipOrderRequest>
            {
                new CreateShipOrderRequest(
                    ShipperId: "Shipper1",
                    KindOfShipOrder: DeliveryMethod.SHIP_ORDER,
                    OrderId: Guid.NewGuid(),
                    ShipDate: DateTime.UtcNow.AddDays(1),
                    ShipOrderDetailRequests: new List<ShipOrderDetailRequest>
                    {
                        new ShipOrderDetailRequest(Guid.NewGuid(), 2, ItemKind.PRODUCT),
                        new ShipOrderDetailRequest(Guid.NewGuid(), 1, ItemKind.SET)
                    }
                ),
                new CreateShipOrderRequest(
                    ShipperId: "Shipper2",
                    KindOfShipOrder: DeliveryMethod.SHIP_ORDER,
                    OrderId: Guid.NewGuid(),
                    ShipDate: DateTime.UtcNow.AddDays(2),
                    ShipOrderDetailRequests: new List<ShipOrderDetailRequest>
                    {
                        new ShipOrderDetailRequest(Guid.NewGuid(), 3, ItemKind.PRODUCT)
                    }
                ),
                new CreateShipOrderRequest(
                    ShipperId: "Shipper3",
                    KindOfShipOrder: DeliveryMethod.RETURN_PRODUCT,
                    OrderId: Guid.NewGuid(),
                    ShipDate: DateTime.UtcNow.AddDays(3),
                    ShipOrderDetailRequests: new List<ShipOrderDetailRequest>
                    {
                        new ShipOrderDetailRequest(Guid.NewGuid(), 1, ItemKind.PRODUCT),
                        new ShipOrderDetailRequest(Guid.NewGuid(), 2, ItemKind.SET)
                    }
                )
            };

        var shipOrders = shipOrderRequests.Select(request => ShipOrder.Create("System", request)).ToList();
        shipOrders.FirstOrDefault().UpdateAccepted("dihson103");

        _context.ShipOrders.AddRange(shipOrders);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByIdAndStatusIsNotDoneAsync_ShouldReturnShipOrder_WhenShipOrderExistsAndIsNotDone()
    {
        // Arrange
        var expectedShipOrder = _context.ShipOrders.First(s => !s.IsAccepted);

        // Act
        var result = await _shipOrderRepository.GetByIdAndStatusIsNotDoneAsync(expectedShipOrder.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedShipOrder.Id, result.Id);
        Assert.False(result.IsAccepted);
    }

    [Fact]
    public async Task GetByIdAndStatusIsNotDoneAsync_ShouldReturnNull_WhenShipOrderExistsAndIsDone()
    {
        // Arrange
        var shipOrder = _context.ShipOrders.FirstOrDefault(s => s.IsAccepted);

        // Act
        var result = await _shipOrderRepository.GetByIdAndStatusIsNotDoneAsync(shipOrder.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAndStatusIsNotDoneAsync_ShouldReturnNull_WhenShipOrderDoesNotExist()
    {
        // Act
        var result = await _shipOrderRepository.GetByIdAndStatusIsNotDoneAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
