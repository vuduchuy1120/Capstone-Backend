using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipmentDetails;

public class ShipmentDetailNotFoundException : MyException
{
    public ShipmentDetailNotFoundException() : base(
        (int) HttpStatusCode.NotFound, 
        "Không tìm thấy chi tiết đơn giao hàng")
    {
    }
}
