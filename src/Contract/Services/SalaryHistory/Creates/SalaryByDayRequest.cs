namespace Contract.Services.SalaryHistory.Creates;

public record SalaryByDayRequest
(
    decimal Salary,
    string StartDate
    );