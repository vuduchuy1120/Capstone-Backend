using Application.Abstractions.Data;
using Domain.Entities;

namespace Persistence.Repositories;

public class MonthlyEmployeeSalaryRepository : IMonthlyEmployeeSalaryRepository
{
    private readonly AppDbContext _context;
    public MonthlyEmployeeSalaryRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddRange(List<MonthlyEmployeeSalary> monthlyEmployeeSalaries)
    {
        _context.MonthlyEmployeeSalaries.AddRange(monthlyEmployeeSalaries);
    }
}
