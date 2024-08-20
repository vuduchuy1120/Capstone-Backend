using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Orders;

public class OrderNotFoundException : MyException
{
    public OrderNotFoundException(Guid id)
       : base(400, $"Không tìm thấy đơn hàng có id: {id}")
    {
    }

    public OrderNotFoundException()
        : base(400, $"Không tìm thấy đơn hàng")
    {
    }
}
