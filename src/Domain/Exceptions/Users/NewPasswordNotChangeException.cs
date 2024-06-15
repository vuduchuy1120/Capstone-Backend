using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class NewPasswordNotChangeException : MyException
{
    public NewPasswordNotChangeException() : base(
        (int) HttpStatusCode.Conflict, "New password can not be the same with old password")
    {
    }
}
