using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ShipmentBadRequestException : MyException
{
    public ShipmentBadRequestException(string message = "Bad request")
        : base((int) HttpStatusCode.BadRequest, message)
    {
    }
}
