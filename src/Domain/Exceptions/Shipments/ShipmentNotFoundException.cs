using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ShipmentNotFoundException : MyException
{
    public ShipmentNotFoundException() : base(
        (int) HttpStatusCode.NotFound, 
        "Shipment is not found")
    {
    }
}
