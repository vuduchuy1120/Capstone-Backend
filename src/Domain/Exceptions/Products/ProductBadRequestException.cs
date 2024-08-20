using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Products;

public class ProductBadRequestException : MyException
{
    public ProductBadRequestException(string message = "Product Bad request") 
        : base((int) HttpStatusCode.BadRequest, message)
    {
    }
}
