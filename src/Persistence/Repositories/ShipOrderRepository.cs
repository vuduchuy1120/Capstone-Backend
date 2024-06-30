
using Application.Abstractions.Data;
using Domain.Entities;

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
}
