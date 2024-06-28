using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ShipDetailItemNullException : MyException
{
    public ShipDetailItemNullException() : base(
        (int) HttpStatusCode.BadRequest, "Item null exception")
    {
    }
}
