using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserDoNotLoggedInException : MyException
{
    public UserDoNotLoggedInException() : base(
        (int) HttpStatusCode.Unauthorized, 
        "Không tìm thấy userId logged in")
    {
    }
}
