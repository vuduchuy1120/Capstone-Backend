namespace Contract.Services.MonthlyCompanySalary.ShareDtos;

public record MonthlyCompanySalaryResponse
(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    string DirectorName,
    decimal Salary,
    StatusSalary Status,
    string? Note
    );
