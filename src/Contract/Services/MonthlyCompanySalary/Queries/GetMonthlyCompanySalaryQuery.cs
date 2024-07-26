using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.MonthlyCompanySalary.ShareDtos;

namespace Contract.Services.MonthlyCompanySalary.Queries;

public record GetMonthlyCompanySalaryQuery
(
    string? SearchCompany,
    int Month,
    int Year,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<MonthlyCompanySalaryResponse>>>;
