using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Products;

public class ProductNotFoundException : MyException
{
    public ProductNotFoundException() : base((int) HttpStatusCode.NotFound, "Không tìm thấy sản phẩm")
    {
    }
}
