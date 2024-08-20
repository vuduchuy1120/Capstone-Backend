namespace Contract.Services.SalaryHistory.Creates;

public record SalaryOverTimeRequest
(
    decimal Salary,
    string StartDate
    );