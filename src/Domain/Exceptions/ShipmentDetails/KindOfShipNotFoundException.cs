using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipmentDetails;

public class KindOfShipNotFoundException : MyException
{
    public KindOfShipNotFoundException() : base(
        (int) HttpStatusCode.NotFound,
        "Kind of ship not found exception")
    {
    }
}
