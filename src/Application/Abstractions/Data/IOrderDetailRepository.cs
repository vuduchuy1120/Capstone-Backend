using Contract.Services.OrderDetail.Queries;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IOrderDetailRepository
{
    void Add(OrderDetail orderDetail);
    void AddRange(List<OrderDetail> orderDetails);
    void UpdateRange(List<OrderDetail> orderDetails);
    void DeleteRange(List<OrderDetail> orderDetails);
    Task<OrderDetail> GetOrderDetailByIdAsync(Guid id);
    Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid id);
    Task<bool> IsOrderDetailExistedAsync(Guid id);
    Task<bool> IsAllOrderDetailProductIdsExistedAsync(Guid orderId, List<Guid?> productIds);
    Task<bool> IsAllOrderDetailSetIdsExistedAsync(Guid orderId, List<Guid?> setIds);
    //Task<(List<OrderDetail>?, int)> SearchByOrderId(GetOrderDetailsByOrderIdQuery request);
}
