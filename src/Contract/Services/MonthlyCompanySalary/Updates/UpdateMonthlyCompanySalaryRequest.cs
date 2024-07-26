using Contract.Services.MonthlyCompanySalary.ShareDtos;

namespace Contract.Services.MonthlyCompanySalary.Updates;

public record UpdateMonthlyCompanySalaryRequest
(
    Guid Id,
    StatusSalary Status,
    string? Note
    );