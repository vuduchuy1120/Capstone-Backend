using Contract.Services.Order.Queries;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IOrderRepository
{
    void AddOrder(Order order);
    void UpdateOrder(Order order);
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<List<Order>> GetOrdersByCompanyIdAsync(Guid companyId);
    Task<(List<Order>?, int)> SearchOrdersAsync(SearchOrderQuery request);
    Task<bool> IsOrderExist(Guid id);

}
