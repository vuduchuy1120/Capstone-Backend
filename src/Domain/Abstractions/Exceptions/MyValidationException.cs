using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Abstractions.Exceptions;

public class MyValidationException : MyException
{
    public MyValidationException(object error) : base(
        (int) HttpStatusCode.BadRequest, 
        "Validation error",
        error)
    {
    }
}
