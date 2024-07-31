using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipOrder;

public class ShipOrderBadRequestException : MyException
{
    public ShipOrderBadRequestException(string message = "Shiporder bad request")
    : base((int)HttpStatusCode.BadRequest, message)
    {
    }
}
