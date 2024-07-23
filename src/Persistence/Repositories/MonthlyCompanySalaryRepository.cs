using Application.Abstractions.Data;
using Domain.Entities;

namespace Persistence.Repositories;

public class MonthlyCompanySalaryRepository : IMonthlyCompanySalaryRepository
{
    private readonly AppDbContext _context;
    public MonthlyCompanySalaryRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddRange(List<MonthlyCompanySalary> monthlyCompanySalaries)
    {
        _context.MonthlyCompanySalaries.AddRange(monthlyCompanySalaries);
    }
}
