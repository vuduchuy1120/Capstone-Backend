using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipOrder;

public class QuantityNotValidException : MyException
{
    public QuantityNotValidException(string message = "Số lượng sản phẩm đang không chính xác") 
        : base((int) HttpStatusCode.BadRequest, message)
    {
    }
}
