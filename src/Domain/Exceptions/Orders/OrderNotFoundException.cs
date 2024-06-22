using Domain.Abstractions.Exceptions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
