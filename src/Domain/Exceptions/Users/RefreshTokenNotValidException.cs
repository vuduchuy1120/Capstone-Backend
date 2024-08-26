using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class RefreshTokenNotValidException : MyException
{
    public RefreshTokenNotValidException(string message = "Không tìm thấy userId trong token") : base(
        (int) HttpStatusCode.Forbidden, message)
    {
    }
}
