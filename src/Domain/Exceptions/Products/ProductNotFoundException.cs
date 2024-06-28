using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Products;

public class ProductNotFoundException : MyException
{
    public ProductNotFoundException() : base((int) HttpStatusCode.NotFound, "Product is not found")
    {
    }
}
