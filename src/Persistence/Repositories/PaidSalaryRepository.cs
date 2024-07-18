using Application.Abstractions.Data;
using Contract.Services.PaidSalary.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class PaidSalaryRepository : IPaidSalaryRepository
{
    private readonly AppDbContext _context;
    public PaidSalaryRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddPaidSalary(PaidSalary paidSalary)
    {
        _context.PaidSalaries.Add(paidSalary);
    }

    public async Task<(List<PaidSalary>, int)> GetPaidSalariesByUserIdAsync(GetPaidSalaryByUserIdQuery request)
    {
        var query = _context.PaidSalaries
            .Include(ps => ps.User)
            .Where(ps => ps.User.Id == request.UserId && ps.User.IsActive);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var paidSalaries = await query
            .OrderByDescending(ps => ps.CreatedDate)
            .Skip(request.PageSize * (request.PageIndex - 1))
            .Take(request.PageSize)
            .ToListAsync();

        return (paidSalaries, totalPages);

    }

    public async Task<PaidSalary> GetPaidSalaryById(Guid id)
    {
        return await _context.PaidSalaries.Include(u => u.User).Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> IsPaidSalaryExistsAsync(Guid Id)
    {
        return await _context.PaidSalaries.AnyAsync(x => x.Id == Id);
    }

    public void UpdatePaidSalary(PaidSalary paidSalary)
    {
        _context.PaidSalaries.Update(paidSalary);
    }

}
