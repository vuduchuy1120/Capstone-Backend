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

    public void Delete(OrderDetail orderDetail)
    {
        _context.OrderDetails.Remove(orderDetail);
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
        return await _context.OrderDetails
            .Include(x => x.Product).ThenInclude(x => x.Images)
            .Include(x => x.Set).ThenInclude(x => x.SetProducts).ThenInclude(x => x.Product).ThenInclude(x => x.Images)
            .Where(x => x.OrderId.Equals(id)).ToListAsync();
    }

    public async Task<bool> IsAllOrderDetailProductIdsExistedAsync(Guid orderId, List<Guid?> productIds)
    {
        if (productIds == null || !productIds.Any())
        {
            throw new ArgumentException("ProductIds must contain at least one ID.", nameof(productIds));
        }

        var query = _context.OrderDetails.Where(x => x.OrderId.Equals(orderId) && productIds.Contains(x.ProductId));

        var count = await query.CountAsync();

        return count == productIds.Count();

    }

    public async Task<bool> IsAllOrderDetailSetIdsExistedAsync(Guid orderId, List<Guid?> setIds)
    {
        if (setIds == null || !setIds.Any())
        {
            throw new ArgumentException("SetIds must contain at least one ID.", nameof(setIds));
        }
        var query = _context.OrderDetails.Where(x => x.OrderId.Equals(orderId) && setIds.Contains(x.SetId));
        var count = await query.CountAsync();
        return (count == setIds.Count());
    }

    public async Task<bool> IsOrderDetailExistedAsync(Guid id)
    {
        return await _context.OrderDetails.AnyAsync(x => x.Id.Equals(id));
    }

    //public async Task<(List<OrderDetail>?, int)> SearchByOrderId(GetOrderDetailsByOrderIdQuery request)
    //{
    //    var query = _context.OrderDetails
    //    .Include(x => x.Product).ThenInclude(x => x.Images)
    //    .Include(x => x.Set)
    //    .Where(x => x.OrderId.Equals(request.OrderId))
    //    .AsQueryable();

    //    var totalItems = await query.CountAsync();

    //    int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

    //    var orderDetails = await query
    //    .Skip((request.PageIndex - 1) * request.PageSize)
    //    .Take(request.PageSize)
    //        .ToListAsync();

    //    return (orderDetails, totalPages);
    //}

    public void UpdateRange(List<OrderDetail> orderDetails)
    {
        _context.OrderDetails.UpdateRange(orderDetails);
    }
}
