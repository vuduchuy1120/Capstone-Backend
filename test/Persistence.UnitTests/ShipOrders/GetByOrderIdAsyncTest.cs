using Application.Abstractions.Data;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ShipOrders;

public class GetByOrderIdAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IShipOrderRepository _shipOrderRepository;
    public GetByOrderIdAsyncTest()
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
    public async Task GetByOrderIdAsync_ReturnsEmptyList_WhenNoShipOrdersForOrderId()
    {
        // Arrange
        var nonExistingOrderId = Guid.NewGuid();

        // Act
        var result = await _shipOrderRepository.GetByOrderIdAsync(nonExistingOrderId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetByOrderIdAsync_ReturnsShipOrders_WithAllDetails()
    {
        // Arrange
        var existingOrder = await _context.ShipOrders.FirstAsync();
        var existingOrderId = existingOrder.Id;

        // Act
        var result = await _shipOrderRepository.GetByOrderIdAsync(existingOrderId);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, so =>
        {
            Assert.NotNull(so.Shipper);
            Assert.NotNull(so.ShipOrderDetails);
            Assert.All(so.ShipOrderDetails, sod => Assert.NotNull(sod.Product));
            Assert.All(so.ShipOrderDetails, sod => Assert.NotNull(sod.Set));
        });
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
