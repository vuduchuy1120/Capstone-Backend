using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserDoNotLoggedInException : MyException
{
    public UserDoNotLoggedInException() : base(
        (int) HttpStatusCode.Unauthorized, 
        "Can not find userId in claims")
    {
    }
}
