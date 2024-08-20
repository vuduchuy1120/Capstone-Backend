using Application.Abstractions.Data;
using Contract.Services.SalaryHistory.ShareDtos;
using Domain.Entities;
using MediatR;
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

    public async Task<(List<SalaryHistory>, int)> GetSalaryHistoryByUserId(string userId, SalaryType salaryType, int pageIndex, int pageSize)
    {
        var query = _context.SalaryHistories
             .Where(x => x.UserId == userId && x.SalaryType == salaryType);

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var salaryHistories = await query
            .OrderByDescending(x => x.StartDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (salaryHistories, totalPages);
    }

    public async Task<SalaryHistory> GetSalaryHistoryByUserIdDateAndSalaryType(string userId, DateOnly date, SalaryType salaryType)
    {
        return await _context.SalaryHistories.Include(x=>x.User)
            .Where(x => x.UserId == userId && x.StartDate == date && x.SalaryType == salaryType)
            .FirstOrDefaultAsync();
    }

    public void UpdateRangeSalaryHistory(List<SalaryHistory> salaryHistories)
    {
        _context.SalaryHistories.UpdateRange(salaryHistories);
    }

    public void UpdateSalaryHistory(SalaryHistory salaryHistory)
    {
        _context.SalaryHistories.Update(salaryHistory);
    }
}
