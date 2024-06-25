using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ShipmentStatusNotFoundException : MyException
{
    public ShipmentStatusNotFoundException() : base(
        (int) HttpStatusCode.NotFound, "Không tìm thấy trạng thái giao hàng tương ứng")
    {
    }
}
