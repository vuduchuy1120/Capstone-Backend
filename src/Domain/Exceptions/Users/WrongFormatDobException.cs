using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class WrongFormatDobException : MyException
{
    public WrongFormatDobException() : base(
        (int) HttpStatusCode.BadRequest, "Dob sai định dạng")
    {
    }
}
