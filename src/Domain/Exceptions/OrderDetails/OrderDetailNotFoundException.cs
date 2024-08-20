using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.OrderDetails;

public class OrderDetailNotFoundException : MyException
{
    public OrderDetailNotFoundException(int id)
       : base(400, $"Không tìm thấy chi tiết đơn hàng có id: {id}")
    {
    }

    public OrderDetailNotFoundException()
        : base(400, $"Không tìm thấy chi tiết đơn hàng")
    {
    }
}
