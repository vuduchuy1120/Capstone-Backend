using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IMonthlyEmployeeSalaryRepository
{
    void AddRange(List<MonthlyEmployeeSalary> monthlyEmployeeSalaries);
    Task<MonthlyEmployeeSalary> GetMonthlyEmployeeSalaryByUserIdAsync(string userId, int month, int year);

}
