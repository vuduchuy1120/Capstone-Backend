using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class RefreshTokenNotValidException : MyException
{
    public RefreshTokenNotValidException() : base(
        (int) HttpStatusCode.Forbidden, "UserId not found in token")
    {
    }
}
