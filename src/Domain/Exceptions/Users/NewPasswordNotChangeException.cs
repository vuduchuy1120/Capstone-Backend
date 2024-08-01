using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class NewPasswordNotChangeException : MyException
{
    public NewPasswordNotChangeException() : base(
        (int) HttpStatusCode.Conflict, "Mật khẩu mới không được giống với mật khẩu cũ")
    {
    }
}
