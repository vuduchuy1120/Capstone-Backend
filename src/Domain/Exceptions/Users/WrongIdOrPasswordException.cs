using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class WrongIdOrPasswordException : MyException
{
    public WrongIdOrPasswordException()
        : base((int)HttpStatusCode.Unauthorized, "Wrong id or password")
    {
    }
}
