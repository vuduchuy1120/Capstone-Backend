using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipmentDetails;

public class ShipmentDetailNotFoundException : MyException
{
    public ShipmentDetailNotFoundException() : base(
        (int) HttpStatusCode.NotFound, 
        "Shipment detail is not found")
    {
    }
}
