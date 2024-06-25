using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserNotPermissionException : MyException
{
    public UserNotPermissionException() : base(
        (int)HttpStatusCode.Forbidden, "You don't have permission to perform this action")
    {
    }

    public UserNotPermissionException(string msg)
        : base((int)HttpStatusCode.Forbidden, $"{msg}")
    {
    }
}
