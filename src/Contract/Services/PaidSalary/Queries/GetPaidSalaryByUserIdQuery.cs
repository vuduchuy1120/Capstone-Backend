using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.PaidSalary.ShareDtos;

namespace Contract.Services.PaidSalary.Queries;

public record GetPaidSalaryByUserIdQuery
(
    string UserId,
    string UserIdClaim,
    string roleNameClaim,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<PaidSalaryResponse>>>;
