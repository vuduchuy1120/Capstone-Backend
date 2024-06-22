using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IOrderDetailRepository
{
    void Add(OrderDetail orderDetail);
    void AddRange(List<OrderDetail> orderDetails);
    void UpdateRange(List<OrderDetail> orderDetails);
    //DeleteRange
    void DeleteRange(List<OrderDetail> orderDetails);
    Task<OrderDetail> GetOrderDetailByIdAsync(Guid id);
    Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid id);
    // check order detail exist
    Task<bool> IsOrderDetailExistedAsync(Guid id);

}
