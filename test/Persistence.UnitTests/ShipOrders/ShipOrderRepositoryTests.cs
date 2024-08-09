using Application.Abstractions.Data;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ShipOrders;

public class ShipOrderRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IShipOrderRepository _shipOrderRepository;

    public ShipOrderRepositoryTests()
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
    public async Task Add_ShipOrderIsAdded()
    {
        // Arrange
        var shipOrderRequest = new CreateShipOrderRequest(
            "shipper3",
            DeliveryMethod.RETURN_PRODUCT,
            Guid.NewGuid(),
            DateTime.UtcNow,
            new List<ShipOrderDetailRequest>
            {
                    new ShipOrderDetailRequest(Guid.NewGuid(), 10, ItemKind.PRODUCT)
            }
        );
        var newShipOrder = ShipOrder.Create("user3", shipOrderRequest);

        // Act
        _shipOrderRepository.Add(newShipOrder);
        await _context.SaveChangesAsync();

        // Assert
        var shipOrderInDb = await _context.ShipOrders
            .Include(s => s.ShipOrderDetails)
            .FirstOrDefaultAsync(s => s.Id == newShipOrder.Id);

        Assert.NotNull(shipOrderInDb);
        Assert.Equal(newShipOrder.Id, shipOrderInDb.Id);
        Assert.Equal(newShipOrder.CreatedBy, shipOrderInDb.CreatedBy);
        Assert.Equal(newShipOrder.CreatedDate, shipOrderInDb.CreatedDate);
        Assert.Equal(newShipOrder.OrderId, shipOrderInDb.OrderId);
        Assert.Equal(newShipOrder.ShipperId, shipOrderInDb.ShipperId);
        Assert.Equal(newShipOrder.ShipDate, shipOrderInDb.ShipDate);
        Assert.Equal(newShipOrder.Status, shipOrderInDb.Status);
        Assert.Equal(newShipOrder.DeliveryMethod, shipOrderInDb.DeliveryMethod);
        Assert.Equal(newShipOrder.IsAccepted, shipOrderInDb.IsAccepted);
        Assert.Equal(newShipOrder.ShipOrderDetails.Count, shipOrderInDb.ShipOrderDetails.Count);
    }

    [Fact]
    public async Task Add_DuplicateShipOrder_ThrowsException()
    {
        // Arrange
        var existingShipOrder = await _context.ShipOrders.FirstAsync();
        var newShipOrder = existingShipOrder;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            _shipOrderRepository.Add(newShipOrder);
            await _context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task Add_Null_ThrowsException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _shipOrderRepository.Add(null);
            await _context.SaveChangesAsync();
        });
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}