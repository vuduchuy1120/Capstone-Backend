using Domain.Abstractions.Exceptions.Base;

namespace Domain.Exceptions.MonthlyCompanySalaries;

public class MonthlyCompanySalaryNotFoundException : MyException
{
    public MonthlyCompanySalaryNotFoundException(int month, int year)
    : base(400, $"Không tìm thấy thông tin lương tháng {month}/{year} của công ty này!")
    {
    }
}
