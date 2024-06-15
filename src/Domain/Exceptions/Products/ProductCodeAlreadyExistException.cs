using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Products;

public class ProductCodeAlreadyExistException : MyException
{
    public ProductCodeAlreadyExistException(string code) : base(
        (int) HttpStatusCode.BadRequest, 
        $"Product code: {code} already exist")
    {
    }
}
