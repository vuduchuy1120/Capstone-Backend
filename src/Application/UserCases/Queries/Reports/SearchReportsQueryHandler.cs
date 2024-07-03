using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Report.Queries;
using Contract.Services.Report.ShareDtos;
using Domain.Exceptions.Users;

namespace Application.UserCases.Queries.Reports;

public class SearchReportsQueryHandler
    (IReportRepository _reportRepository,
    IMapper _mapper
    ) : IQueryHandler<SearchReportsWithClaimsQuery, SearchResponse<List<ReportResponse>>>
{
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

        var data = _mapper.Map<List<ReportResponse>>(reports);
        var searchResponse = new SearchResponse<List<ReportResponse>>(request.SearchRequest.PageIndex, totalPage, data);
        return Result.Success<SearchResponse<List<ReportResponse>>>.Get(searchResponse);
    }
}
