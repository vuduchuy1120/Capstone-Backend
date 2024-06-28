using Application.Abstractions.Data;
using Application.Abstractions.Shared.Utils;
using Application.Utils;
using Contract.Services.Order.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddOrder(Order order)
    {
        _context.Orders.Add(order);
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public Task<List<Order>> GetOrdersByCompanyIdAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsOrderExist(Guid id)
    {
        return await _context.Orders.AnyAsync(x => x.Id.Equals(id));
    }

    public async Task<(List<Order>?, int)> SearchOrdersAsync(SearchOrderQuery request)
    {
        var query = _context.Orders.Include(o => o.Company).AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.CompanyName))
        {
            query = query.Where(x => x.Company.NameUnAccent.Equals(StringUtils.RemoveDiacritics(request.CompanyName)));
        }
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(x => x.Status.Equals(request.Status));
        }
        if (!string.IsNullOrWhiteSpace(request.StartOrder))
        {
            var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.StartOrder);
            query = query.Where(x => x.StartOrder == formatedDate);
        }
        if (!string.IsNullOrWhiteSpace(request.EndOrder))
        {
            var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.EndOrder);
            query = query.Where(x => x.EndOrder == formatedDate);
        }
        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var orders = await query
            .OrderByDescending(order => order.StartOrder)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (orders, totalPages);





    }

    public void UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
    }
}
