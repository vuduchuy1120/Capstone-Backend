using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<MonthlyEmployeeSalary> GetMonthlyEmployeeSalaryByUserIdAsync(string userId, int month, int year)
    {
        var query = await _context.MonthlyEmployeeSalaries
            .AsNoTracking()
            .Include(mes => mes.User)
                .ThenInclude(u => u.Attendances)
            .Include(mes => mes.User)
                .ThenInclude(u => u.EmployeeProducts)
                .ThenInclude(ep => ep.Product)
                .ThenInclude(p => p.ProductPhaseSalaries)
                .ThenInclude(pp => pp.Phase)
            .Where(u => u.User.Id == userId && u.User.IsActive == true && u.User.Attendances.Any(a => a.Date.Month == month && a.Date.Year == year) ||
                                   u.User.EmployeeProducts.Any(ep => ep.Date.Month == month && ep.Date.Year == year)).ToListAsync();
        return query.FirstOrDefault();
    }
}
