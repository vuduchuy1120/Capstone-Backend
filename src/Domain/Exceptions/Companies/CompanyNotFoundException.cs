using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Companies;

public class CompanyNotFoundException : MyException
{
    public CompanyNotFoundException(Guid id)
       : base(400, $"Không tìm thấy công ty có id: {id}")
    {
    }

    public CompanyNotFoundException()
        : base(400, $"Không tìm thấy công ty nào phù hợp")
    {
    }

}
