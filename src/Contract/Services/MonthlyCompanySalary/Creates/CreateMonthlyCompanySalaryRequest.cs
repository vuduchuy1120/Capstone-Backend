namespace Contract.Services.MonthlyCompanySalary.Creates;

public record CreateMonthlyCompanySalaryRequest
(
    Guid CompanyId,
    int Month,
    int Year,
    decimal Salary
    );
