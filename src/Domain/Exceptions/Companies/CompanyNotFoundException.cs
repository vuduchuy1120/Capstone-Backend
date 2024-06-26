using Domain.Abstractions.Exceptions.Base;
using System.Net;

namespace Domain.Exceptions.Companies;

public class CompanyNotFoundException : MyException
{
    public CompanyNotFoundException(Guid id)
       : base(400, $"Can not found Company has id: {id}")
    {
    }

    public CompanyNotFoundException()
        : base(400, $"Search Company not found")
    {
    }

}
