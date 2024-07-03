using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Report.ShareDtos;

namespace Contract.Services.Report.Queries;

public record SearchReportsWithClaimsQuery
(
    SearchReportsQuery SearchRequest,
    string RoleNameClaims,
    Guid CompanyIdClaims,
    string UserIdClaims
    ): IQuery<SearchResponse<List<ReportResponse>>>;
