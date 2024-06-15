using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserAlreadyExistedException : MyException
{
    public UserAlreadyExistedException(string id)
        : base((int)HttpStatusCode.Conflict, $"User has id: {id} is already existed.")
    {
    }
}
