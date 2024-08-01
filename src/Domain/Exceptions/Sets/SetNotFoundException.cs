using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Sets;

public class SetNotFoundException : MyException
{
    public SetNotFoundException(Guid id) : base(
        (int) HttpStatusCode.NotFound, 
        $"Không tìm thấy bộ sản phẩm có id: {id}")
    {
    }

    public SetNotFoundException() : base(
        (int)HttpStatusCode.NotFound,
        "Không tìm thấy bộ sản phẩm")
    {
    }
}
