using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class VerifyCodeNotValidException : MyException
{
    public VerifyCodeNotValidException() : base(
        (int) HttpStatusCode.BadRequest, "Verify code is not valid")
    {
    }
}
