using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserAlreadyExistedException : MyException
{
    public UserAlreadyExistedException(string id)
        : base((int)HttpStatusCode.Conflict, $"Người dùng có id: {id} đã tồn tại.")
    {
    }
}
