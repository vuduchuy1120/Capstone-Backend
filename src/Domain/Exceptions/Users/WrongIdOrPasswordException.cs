using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class WrongIdOrPasswordException : MyException
{
    public WrongIdOrPasswordException(int status = (int)HttpStatusCode.Unauthorized, string message = "Sai tài khoản hoặc mật khẩu")
        : base(status, message)
    {
    }
}
