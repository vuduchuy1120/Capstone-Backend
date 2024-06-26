using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Companies;

public class CompanyNotFoundException : MyException
{
    public CompanyNotFoundException() : base(
        (int) HttpStatusCode.NotFound, "Không tìm thấy công ty theo yêu cầu")
    {
    }
}
