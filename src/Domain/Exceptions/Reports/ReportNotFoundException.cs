using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.Reports;

public class ReportNotFoundException : MyException
{
    public ReportNotFoundException(Guid id)
       : base(400, $"Không tìm thấy report có id: {id}")
    {
    }

    public ReportNotFoundException()
        : base(400, $"Không tìm thấy report")
    {
    }
}