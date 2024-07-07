using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Report.Queries;
using Contract.Services.Report.ShareDtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        return await _context.Reports.Include(u => u.User).FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<List<Report>> GetReportsByUserIdAsync(Guid userId)
    {
        return await _context.Reports.Where(x => x.UserId.Equals(userId)).ToListAsync();
    }

    public async Task<bool> IsCanGetReportByIdAsync(Guid reportId, Guid companyIdClaim, string userClaim, string roleNameClaim)
    {
        var report = await _context.Reports.Include(u => u.User).FirstOrDefaultAsync(x => x.Id.Equals(reportId));

        if (roleNameClaim != "MAIN_ADMIN" && roleNameClaim != "BRANCH_ADMIN")
        {
            return report.UserId == userClaim;
        }
        if (roleNameClaim == "BRANCH_ADMIN")
        {
            return report.User.CompanyId == companyIdClaim;
        }
        return true;
    }

    public async Task<bool> IsCanUpdateReport(Guid reportId, Guid companyIdClaim)
    {
        var report = await _context.Reports.Include(u => u.User).FirstOrDefaultAsync(x => x.Id.Equals(reportId));
        return report.User.CompanyId == companyIdClaim;
    }

    public async Task<bool> IsReportExisted(Guid id)
    {
        return await _context.Reports.AnyAsync(x => x.Id.Equals(id));
    }

    public async Task<(List<Report>?, int)> SearchReports(SearchReportsQuery request)
    {
        var query = _context.Reports
            .Include(x => x.User)
            .Include(x => x.User.Company)
            .AsNoTracking()
            .Where(x => x.User.CompanyId == request.CompanyId);

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            query = query.Where(x => x.UserId == request.UserId);
        }
        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            query = query.Where(x =>
            x.User.FirstName.ToLower().Trim().Contains(request.UserName.ToLower().Trim()) ||
            x.User.LastName.ToLower().Trim().Contains(request.UserName.ToLower().Trim()) ||
            (x.User.FirstName.ToLower().Trim() + " " + x.User.LastName.ToLower().Trim()).Contains(request.UserName.ToLower().Trim()));
        }
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<StatusReport>(request.Status, true, out var statusType))
            {
                query = query.Where(status => status.Status == statusType);
            }
        }
        if (!string.IsNullOrWhiteSpace(request.ReportType))
        {
            if (Enum.TryParse<ReportType>(request.ReportType, true, out var reportType))
            {
                query = query.Where(r => r.ReportType == reportType);
            }
        }
        if (!string.IsNullOrWhiteSpace(request.StartDate))
        {
            var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.StartDate);
            query = query.Where(x => DateOnly.FromDateTime(x.CreatedDate) >= formatedDate);
        }
        if (!string.IsNullOrWhiteSpace(request.EndDate))
        {
            var formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.EndDate);
            query = query.Where(x => DateOnly.FromDateTime(x.CreatedDate) <= formatedDate);
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var reports = await query
            .OrderByDescending(report => report.CreatedDate)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (reports, totalPages);
    }

    public void Update(Report report)
    {
        _context.Reports.Update(report);
    }

}
