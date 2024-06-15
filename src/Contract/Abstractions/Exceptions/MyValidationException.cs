using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Contract.Abstractions.Exceptions;

public class MyValidationException : MyException
{
    public MyValidationException(object error) : base(
        (int)HttpStatusCode.BadRequest,
        "Validation error",
        error)
    {
    }
}