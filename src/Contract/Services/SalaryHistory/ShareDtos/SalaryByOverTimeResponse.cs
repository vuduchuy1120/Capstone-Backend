namespace Contract.Services.SalaryHistory.ShareDtos;

public record SalaryByOverTimeResponse
(
    decimal Salary,
    DateOnly StartDate    
    );
