using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Sets;

public class SetBadRequestException : MyException
{
    public SetBadRequestException(string message = "Set bad request") : base((int)HttpStatusCode.BadRequest, message)
    {
    }
}
