using Application.Abstractions.Data;
using Contract.Services.ShipmentDetail.Share;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class ShipmentDetailRepository : IShipmentDetailRepository
{
    private readonly AppDbContext _context;

    public ShipmentDetailRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddRange(List<ShipmentDetail> shipmentDetails)
    {
        _context.ShipmentDetails.AddRange(shipmentDetails);
    }

    public async Task<List<ShipmentDetail>> GetByShipmentIdAndIdsAsync(Guid shipmentId, List<Guid> shipDetailIds)
    {
        return await _context.ShipmentDetails
            .Where(s => s.ShipmentId == shipmentId && shipDetailIds.Contains(s.Id))
            .ToListAsync();
    }

    public async Task<List<ShipmentDetail>> GetShipmentDetailByShipmentIdAndProductPhaseType(Guid shipmentId, ProductPhaseType productPhaseType)
    {
        var query = await _context.ShipmentDetails
            .Include(s => s.Shipment)
            .Where(s => s.ShipmentId == shipmentId && s.ProductPhaseType == productPhaseType).ToListAsync();

        return query;
    }

    public async Task<bool> IsAllShipDetailIdAndShipmentIdValidAsync(Guid shipmentId, List<Guid> shipDetailIds)
    {
        var numberExist = await _context.ShipmentDetails
            .CountAsync(s => s.ShipmentId == shipmentId && shipDetailIds.Contains(s.Id));

        return numberExist == shipDetailIds.Count;
    }

    public void UpdateRange(List<ShipmentDetail> shipmentDetails)
    {
        _context.ShipmentDetails.UpdateRange(shipmentDetails);
    }
}
