using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Products;

public class ProductIdConflictException : MyException
{
    public ProductIdConflictException() : base(
        (int) HttpStatusCode.BadRequest, 
        "Id của sản phẩm không trùng khớp")
    {
    }
}
