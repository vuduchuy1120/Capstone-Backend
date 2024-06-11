using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Files;

public class UploadFileException : MyException
{
    public UploadFileException(string message) : base((int) HttpStatusCode.BadRequest, message)
    {
    }
}
