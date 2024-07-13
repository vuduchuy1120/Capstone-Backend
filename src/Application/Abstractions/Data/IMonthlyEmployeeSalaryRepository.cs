using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IMonthlyEmployeeSalaryRepository
{
    void AddRange(List<MonthlyEmployeeSalary> monthlyEmployeeSalaries);
}
