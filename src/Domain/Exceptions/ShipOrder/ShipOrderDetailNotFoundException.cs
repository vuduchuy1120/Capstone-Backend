using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipOrder;

public class ShipOrderDetailNotFoundException : MyException
{
    public ShipOrderDetailNotFoundException() 
        : base((int) HttpStatusCode.NotFound, "Có 1 đơn giao không tìm thấy sản phẩm trong đơn giao")
    {
    }
}
