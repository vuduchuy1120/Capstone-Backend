using Contract.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Users;

public class UserNotFoundException : MyException
{
    public UserNotFoundException(string id)
        : base(400, $"Can not found user has id: {id}")
    {
    }

    public UserNotFoundException()
        : base(400, $"Search user not found")
    {
    }
}
