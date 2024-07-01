using Application.Abstractions.Data;
using Domain.Entities;

namespace Persistence.Repositories;

public class SalaryHistoryRepository : ISalaryHistoryRepository
{
    private readonly AppDbContext _context;
    public SalaryHistoryRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddSalaryHistory(SalaryHistory salaryHistory)
    {
        _context.SalaryHistories.Add(salaryHistory);
    }
}
