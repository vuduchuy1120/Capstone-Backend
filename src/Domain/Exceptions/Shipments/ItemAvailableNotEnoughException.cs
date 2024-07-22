using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Shipments;

public class ItemAvailableNotEnoughException : MyException
{
    public ItemAvailableNotEnoughException(string message = "Số lượng sản phẩm sẵn sàng để giao không đủ") : base(
        (int) HttpStatusCode.BadRequest, message)
    {
    }
}
