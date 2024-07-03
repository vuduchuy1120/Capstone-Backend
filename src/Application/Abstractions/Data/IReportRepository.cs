using Contract.Services.Report.Queries;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IReportRepository
{
    void Add(Report report);
    void Update(Report report);
    Task<Report> GetReportByIdAsync(Guid id);
    Task<List<Report>> GetReportsByUserIdAsync(Guid userId);
    Task<bool> IsReportExisted(Guid id);
    Task<(List<Report>?, int)> SearchReports(SearchReportsQuery request);
    Task<bool> IsCanUpdateReport(Guid reportId, Guid companyIdClaim);
    Task<bool> IsCanGetReportByIdAsync(Guid reportId, Guid companyIdClaim, string userClaim, string roleNameClaim);

}
