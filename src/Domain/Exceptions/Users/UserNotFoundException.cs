using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class UserNotFoundException : MyException
{
    public UserNotFoundException(string id)
        : base((int) HttpStatusCode.NotFound, $"Can not found user has id: {id}")
    {
    }

    public UserNotFoundException()
        : base((int) HttpStatusCode.NotFound, $"Search user not found")
    {
    }
}
