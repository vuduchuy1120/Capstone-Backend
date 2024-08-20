using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.ShipOrder;

public class ShipOrderDetailDoNotHaveItemIdException : MyException
{
    public ShipOrderDetailDoNotHaveItemIdException() 
        : base((int)HttpStatusCode.NotFound, "Không tìm thấy bất kỳ id vật phẩm nào trong shipOrderDetail")
    {
    }
}
