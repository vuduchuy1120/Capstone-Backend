using Application.Abstractions.Data;
using Application.Abstractions.Shared.Utils;
using Contract.Services.MonthlyCompanySalary.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<MonthlyCompanySalary> GetByIdAsync(Guid id)
    {
        return await _context.MonthlyCompanySalaries.SingleOrDefaultAsync(mcs => mcs.Id == id);
    }

    public Task<MonthlyCompanySalary> GetMonthlyCompanySalaryByCompanyIdMonthAndYear(Guid CompanyId, int Month, int Year)
    {
        return _context.MonthlyCompanySalaries
            .SingleOrDefaultAsync(mcs => mcs.CompanyId == CompanyId && mcs.Month == Month && mcs.Year == Year);
    }

    public async Task<MonthlyCompanySalary> GetMonthlyCompanySalaryByIdAsync(Guid id)
    {
        return await _context.MonthlyCompanySalaries
            .SingleOrDefaultAsync(mcs => mcs.Id == id);
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        return await _context.MonthlyCompanySalaries.AnyAsync(mcs => mcs.Id == id);
    }

    public async Task<bool> IsExistMonthlyCompanySalary(Guid CompanyId, int Month, int Year)
    {
        return await _context.MonthlyCompanySalaries.AnyAsync(mcs => mcs.CompanyId == CompanyId && mcs.Month == Month && mcs.Year == Year);
    }

    public async Task<(List<MonthlyCompanySalary>, int)> SearchMonthlyCompanySalary(GetMonthlyCompanySalaryQuery request)
    {
        var query = _context.MonthlyCompanySalaries
            .Include(mcs => mcs.Company)
            .Where(mcs => mcs.Month == request.Month && mcs.Year == request.Year);

        if (!string.IsNullOrEmpty(request.SearchCompany))
        {
            var search = StringUtils.RemoveDiacritics(request.SearchCompany.ToLower().Trim());
            query = query.Where(mcs => mcs.Company.NameUnAccent.ToLower().Trim().Contains(search) || mcs.Company.Id.Equals(search));
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var items = await query
            .OrderBy(mcs => mcs.Company.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return (items, totalPages);
    }

    public void Update(MonthlyCompanySalary monthlyCompanySalary)
    {
        _context.MonthlyCompanySalaries.Update(monthlyCompanySalary);
    }
}
