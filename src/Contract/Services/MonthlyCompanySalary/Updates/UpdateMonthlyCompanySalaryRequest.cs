using Contract.Services.MonthlyCompanySalary.ShareDtos;

namespace Contract.Services.MonthlyCompanySalary.Updates;

public record UpdateMonthlyCompanySalaryRequest
(
    Guid CompanyId,
    int Month,
    int Year,
    StatusSalary Status,
    string? Note
    );