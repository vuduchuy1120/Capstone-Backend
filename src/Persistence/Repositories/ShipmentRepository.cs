using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class ShipmentRepository : IShipmentRepository
{
    private readonly AppDbContext _context;

    public ShipmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Shipment shipment)
    {
        _context.Add(shipment);
    }

    public async Task<Shipment> GetByIdAsync(Guid shipmentId)
    {
        return await _context.Shipments.SingleOrDefaultAsync(s => s.Id == shipmentId);
    }

    public async Task<bool> IsShipmentIdExistAsync(Guid shipmentId)
    {
        return await _context.Shipments.AnyAsync(s => s.Id == shipmentId);  
    }

    public void Update(Shipment shipment)
    {
        _context.Shipments.Update(shipment);
    }
}
