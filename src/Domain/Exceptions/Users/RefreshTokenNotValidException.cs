using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class RefreshTokenNotValidException : MyException
{
    public RefreshTokenNotValidException() : base(
        (int) HttpStatusCode.Forbidden, "Không tìm thấy userId trong token")
    {
    }
}
