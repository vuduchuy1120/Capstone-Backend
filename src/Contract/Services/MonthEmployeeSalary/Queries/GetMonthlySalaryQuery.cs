using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.MonthEmployeeSalary.ShareDtos;

namespace Contract.Services.MonthEmployeeSalary.Queries;

public record GetMonthlySalaryQuery
(
    string? userId,
    string? fullName,
    int? month,
    int? year,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<MonthlySalaryResponse>>>;
