using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Products;

public class ProductIdConflictException : MyException
{
    public ProductIdConflictException() : base(
        (int) HttpStatusCode.BadRequest, 
        "There is something wrong with update product id")
    {
    }
}
