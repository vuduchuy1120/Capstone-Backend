
using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipOrder;

public class ShipOrderIdConflictException : MyException
{
    public ShipOrderIdConflictException() : base(
        (int) HttpStatusCode.Conflict, "Order id đang không đồng nhất")
    {
    }
}
