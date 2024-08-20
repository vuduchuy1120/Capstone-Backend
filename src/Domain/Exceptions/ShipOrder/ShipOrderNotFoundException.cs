using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipOrder;

public class ShipOrderNotFoundException : MyException
{
    public ShipOrderNotFoundException(string message = "Không tìm thấy đơn giao") : base(
        (int) HttpStatusCode.NotFound, message)
    {
    }
}
