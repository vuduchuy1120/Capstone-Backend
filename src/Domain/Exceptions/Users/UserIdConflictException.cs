using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserIdConflictException : MyException
{
    public UserIdConflictException() : base(
        (int) HttpStatusCode.Conflict, "Mã người dùng không chính xác")
    {
    }
}
