namespace Contract.Services.PaidSalary.Updates;

public record UpdatePaidSalaryRequest
(
    Guid Id,
    decimal Salary,
    string? Note
    );
