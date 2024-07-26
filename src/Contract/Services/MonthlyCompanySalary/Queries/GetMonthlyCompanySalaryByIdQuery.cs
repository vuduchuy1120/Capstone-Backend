using Contract.Abstractions.Messages;
using Contract.Services.MonthlyCompanySalary.ShareDtos;

namespace Contract.Services.MonthlyCompanySalary.Queries;

public record GetMonthlyCompanySalaryByIdQuery
(
    Guid MonthlyCompanySalaryId,
    Guid CompanyId,
    int Month,
    int Year
    ) : IQuery<MonthlyCompanySalaryDetailResponse>;
