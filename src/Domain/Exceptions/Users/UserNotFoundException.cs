using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserNotFoundException : MyException
{
    public UserNotFoundException(string id)
        : base((int) HttpStatusCode.NotFound, $"Không tìm thấy người dùng có id hoặc số điện thoại: {id}")
    {
    }

    public UserNotFoundException()
        : base((int) HttpStatusCode.NotFound, $"Không tìm thấy người dùng")
    {
    }
}
