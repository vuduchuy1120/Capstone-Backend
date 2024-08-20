using Application.Abstractions.Data;
using Domain.Entities;

namespace Persistence.Repositories;

internal class ShipOrderDetailRepository : IShipOrderDetailRepository
{
    private readonly AppDbContext _context;

    public ShipOrderDetailRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddRange(List<ShipOrderDetail> shipOrderDetails)
    {
        _context.ShipOrderDetails.AddRange(shipOrderDetails);
    }
}
