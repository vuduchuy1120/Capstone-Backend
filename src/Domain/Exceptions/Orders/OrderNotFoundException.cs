using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Orders;

public class OrderNotFoundException : MyException
{
    public OrderNotFoundException(Guid id)
       : base(400, $"Can not found Order has id: {id}")
    {
    }

    public OrderNotFoundException()
        : base(400, $"Search Order not found")
    {
    }
}
