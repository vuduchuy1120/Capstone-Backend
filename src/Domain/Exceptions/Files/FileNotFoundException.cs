using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Files;

public class FileNotFoundException : MyException
{
    public FileNotFoundException() : base((int) HttpStatusCode.NotFound, "Không tìm thấy file")
    {
    }
}
