using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Report.Queries;
using Contract.Services.Report.ShareDtos;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.Reports;

public class SearchReportsQueryHandler : IQueryHandler<SearchReportsWithClaimsQuery, SearchResponse<List<ReportResponse>>>
{
    private readonly IReportRepository _reportRepository;
    private readonly ICloudStorage _cloudStorage;

    public SearchReportsQueryHandler(IReportRepository reportRepository, ICloudStorage cloudStorage)
    {
        _reportRepository = reportRepository;
        _cloudStorage = cloudStorage;
    }

    public async Task<Result.Success<SearchResponse<List<ReportResponse>>>> Handle(SearchReportsWithClaimsQuery request, CancellationToken cancellationToken)
    {
        if (request.RoleNameClaims == "BRANCH_ADMIN" && request.CompanyIdClaims != request.SearchRequest.CompanyId)
        {
            throw new UserNotPermissionException("Bạn không có quyền xem báo cáo của cơ sở khác");
        }
        if (request.RoleNameClaims != "MAIN_ADMIN"
            && request.RoleNameClaims != "BRANCH_ADMIN"
            && (request.UserIdClaims != request.SearchRequest.UserId || string.IsNullOrWhiteSpace(request.SearchRequest.UserId)))
        {
            throw new UserNotPermissionException("Bạn không có quyền xem báo cáo của người khác");
        }

        var query = await _reportRepository.SearchReports(request.SearchRequest);
        var reports = query.Item1;
        var totalPage = query.Item2;

        var data = new List<ReportResponse>();
        foreach (var report in reports)
        {
            var avatarUrl = await _cloudStorage.GetSignedUrlAsync(report.User.Avatar);

            var reportResponse = new ReportResponse(
                report.Id,
                report.User.Id,
                report.User.FirstName + " " + report.User.LastName,
                avatarUrl,
                report.Description,
                report.Status,
                report.Status.ToString(),
                report.Status.GetDescription(),
                report.ReportType,
                report.ReportType.ToString(),
                report.ReportType.GetDescription(),
                report.ReplyMessage,
                report.User.CompanyId,
                DateOnly.FromDateTime(report.CreatedDate.Date)
            );

            data.Add(reportResponse);
        }

        var searchResponse = new SearchResponse<List<ReportResponse>>(request.SearchRequest.PageIndex, totalPage, data);
        return Result.Success<SearchResponse<List<ReportResponse>>>.Get(searchResponse);
    }
}
