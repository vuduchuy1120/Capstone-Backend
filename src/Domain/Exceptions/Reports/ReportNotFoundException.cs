using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Reports;

public class ReportNotFoundException : MyException
{
    public ReportNotFoundException(Guid id)
       : base(400, $"Can not found report has id: {id}")
    {
    }

    public ReportNotFoundException()
        : base(400, $"Search report not found")
    {
    }
}