namespace Contract.Services.PaidSalary.Creates;

public record CreatePaidSalaryRequest
(
    string UserId,
    decimal Salary,
    string? Note
    );
