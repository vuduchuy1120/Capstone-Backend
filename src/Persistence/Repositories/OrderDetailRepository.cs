using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class OrderDetailRepository : IOrderDetailRepository
{
    private readonly AppDbContext _context;
    public OrderDetailRepository(AppDbContext context)
    {
        _context = context;
    }
    public void Add(OrderDetail orderDetail)
    {
        _context.OrderDetails.Add(orderDetail);
    }

    public void AddRange(List<OrderDetail> orderDetails)
    {
        _context.OrderDetails.AddRange(orderDetails);
    }

    public void DeleteRange(List<OrderDetail> orderDetails)
    {
        _context.OrderDetails.RemoveRange(orderDetails);
    }

    public async Task<OrderDetail> GetOrderDetailByIdAsync(Guid id)
    {
        return await _context.OrderDetails.FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid id)
    {
        return await _context.OrderDetails.Where(x => x.OrderId.Equals(id)).ToListAsync();
    }

    public async Task<bool> IsOrderDetailExistedAsync(Guid id)
    {
        return await _context.OrderDetails.AnyAsync(x => x.Id.Equals(id));
    }

    public void UpdateRange(List<OrderDetail> orderDetails)
    {
        _context.OrderDetails.UpdateRange(orderDetails);
    }
}
