using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipmentDetails;

public class KindOfShipNotFoundException : MyException
{
    public KindOfShipNotFoundException() : base(
        (int) HttpStatusCode.NotFound,
        "Không tìm thấy loại giao hàng phù hợp")
    {
    }
}
