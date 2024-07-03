
using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class ShipOrderRepository : IShipOrderRepository
{
    private readonly AppDbContext _context;

    public ShipOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(ShipOrder shipOrder)
    {
        _context.ShipOrders.Add(shipOrder);
    }

    public async Task<List<ShipOrder>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.ShipOrders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(shipOrder => shipOrder.Shipper)
            .Include(shipOrder => shipOrder.ShipOrderDetails)
                .ThenInclude(shipOrderDetail => shipOrderDetail.Product)
                    .ThenInclude(p => p.Images)
            .Include(shipOrder => shipOrder.ShipOrderDetails)
                .ThenInclude(shipOrderDetail => shipOrderDetail.Set)
                    .ThenInclude(s => s.SetProducts)
                        .ThenInclude(sp => sp.Product)
                            .ThenInclude(p => p.Images)
            .Where(shipOrder => shipOrder.OrderId == orderId)
            .ToListAsync();
    }
}
