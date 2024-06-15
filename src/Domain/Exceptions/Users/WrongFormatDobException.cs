using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Users;

public class WrongFormatDobException : MyException
{
    public WrongFormatDobException() : base(
        (int) HttpStatusCode.BadRequest, "Dob is wrong format")
    {
    }
}
