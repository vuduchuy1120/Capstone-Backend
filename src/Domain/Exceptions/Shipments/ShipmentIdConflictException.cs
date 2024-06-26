using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ShipmentIdConflictException : MyException
{
    public ShipmentIdConflictException() : base(
        (int) HttpStatusCode.Conflict, "Mã giao hàng đang khác nhau")
    {
    }
}
