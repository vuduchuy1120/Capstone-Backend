using Contract.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Files;

public class UploadFileException : MyException
{
    public UploadFileException() : base((int) HttpStatusCode.BadRequest, "Upload file error")
    {
    }
}
