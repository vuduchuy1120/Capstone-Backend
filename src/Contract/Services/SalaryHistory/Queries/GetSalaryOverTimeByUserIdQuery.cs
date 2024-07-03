using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.SalaryHistory.ShareDtos;

namespace Contract.Services.SalaryHistory.Queries;

public record GetSalaryOverTimeByUserIdQuery
(
    string UserId,
    string RoleName,
    string UserIdClaims,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<SalaryByOverTimeResponse>>>;