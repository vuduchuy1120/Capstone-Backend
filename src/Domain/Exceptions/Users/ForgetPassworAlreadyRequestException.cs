using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class ForgetPassworAlreadyRequestException : MyException
{
    public ForgetPassworAlreadyRequestException() : base(
        (int) HttpStatusCode.BadRequest, "Bạn đã gửi yêu cầu quên mật khẩu")
    {
    }
}
