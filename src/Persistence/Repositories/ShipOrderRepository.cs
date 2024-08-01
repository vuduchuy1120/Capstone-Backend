﻿
using Application.Abstractions.Data;
using Contract.Services.Shipment.Share;
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

    public async Task<ShipOrder> GetByIdAndStatusIsNotDoneAsync(Guid shipOrderId)
    {
        return await _context.ShipOrders
            .Include(s => s.ShipOrderDetails)
            .SingleOrDefaultAsync(s => s.Id == shipOrderId && (s.Status == Status.WAIT_FOR_SHIP || s.Status == Status.SHIPPING));
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

    public async Task<ShipOrder> GetByShipOrderIdAsync(Guid shipOrderId)
    {
        return await _context.ShipOrders
            .Include(s => s.ShipOrderDetails)
            .SingleOrDefaultAsync(s => s.Id == shipOrderId);
    }

    public async Task<bool> IsAnyShipOrderNotDone(Guid orderId)
    {
        return await _context.ShipOrders
            .AnyAsync(s => s.OrderId == orderId && (s.Status == Status.WAIT_FOR_SHIP || s.Status == Status.SHIPPING));
    }

    public async Task<bool> IsExistAnyShipOrder(Guid orderId)
    {
        return await _context.ShipOrders
            .AnyAsync(s => s.OrderId == orderId);
    }

    public async Task<bool> IsShipOrderExistAndInWaitingStatusAsync(Guid shipOrderId)
    {
        return await _context.ShipOrders
            .AnyAsync(s => s.Id == shipOrderId && s.Status == Status.WAIT_FOR_SHIP);
    }

    public void Update(ShipOrder shipOrder)
    {
        _context.ShipOrders.Update(shipOrder);
    }
}
