using Application.Abstractions.Data;
using Contract.Services.SalaryHistory.ShareDtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class SalaryHistoryRepository : ISalaryHistoryRepository
{
    private readonly AppDbContext _context;
    public SalaryHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddRangeSalaryHistory(List<SalaryHistory> salaryHistories)
    {
        _context.SalaryHistories.AddRange(salaryHistories);
    }

    public void AddSalaryHistory(SalaryHistory salaryHistory)
    {
        _context.SalaryHistories.Add(salaryHistory);
    }

    public async Task<SalaryHistory> GetSalaryHistoryByUserIdDateAndSalaryType(string userId, DateOnly date, SalaryType salaryType)
    {
        return await _context.SalaryHistories
            .Where(x => x.UserId == userId && x.StartDate == date && x.SalaryType == salaryType)
            .FirstOrDefaultAsync();
    }

    public void UpdateRangeSalaryHistory(List<SalaryHistory> salaryHistories)
    {
        _context.SalaryHistories.UpdateRange(salaryHistories);
    }
}
