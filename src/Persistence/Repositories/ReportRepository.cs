using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;
    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }
    public void Add(Report report)
    {
        _context.Reports.Add(report);
    }

    public async Task<Report> GetReportByIdAsync(Guid id)
    {
        return await _context.Reports.FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<Report>> GetReportsByUserIdAsync(Guid userId)
    {
        return await _context.Reports.Where(x => x.UserId.Equals(userId)).ToListAsync();
    }

    public async Task<bool> IsReportExisted(Guid id)
    {
        return await _context.Reports.AnyAsync(x => x.Id.Equals(id));
    }

    public void Update(Report report)
    {
        _context.Reports.Update(report);
    }
}
