using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ShipmentAlreadyDoneException : MyException
{
    public ShipmentAlreadyDoneException() : base(
        (int) HttpStatusCode.BadRequest, "Đơn hàng đã hoàn thành không được sửa đổi")
    {
    }
}
