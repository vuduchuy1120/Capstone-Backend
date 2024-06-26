using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ItemAvailableNotEnoughException : MyException
{
    public ItemAvailableNotEnoughException() : base(
        (int) HttpStatusCode.BadRequest, "Số lượng sản phẩm sẵn sàng để giao không đủ")
    {
    }
}
