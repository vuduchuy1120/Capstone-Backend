using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.OrderDetails;

public class OrderDetailNotFoundException : MyException
{
    public OrderDetailNotFoundException(int id)
       : base(400, $"Can not found order detail has orderId: {id}")
    {
    }

    public OrderDetailNotFoundException()
        : base(400, $"Search order detail not found")
    {
    }
}
