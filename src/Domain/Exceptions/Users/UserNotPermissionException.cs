using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserNotPermissionException : MyException
{
    public UserNotPermissionException() : base(
        (int)HttpStatusCode.Forbidden, "Bạn không có quyền để thực hiện hành động này")
    {
    }

    public UserNotPermissionException(string msg)
        : base((int)HttpStatusCode.Forbidden, $"{msg}")
    {
    }
}
