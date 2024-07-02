namespace Contract.Services.SalaryHistory.ShareDtos;

public record SalaryByDayResponse
(
    decimal Salary,
    DateOnly StartDate
    );
