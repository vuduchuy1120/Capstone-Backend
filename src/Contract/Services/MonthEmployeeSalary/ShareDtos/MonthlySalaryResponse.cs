namespace Contract.Services.MonthEmployeeSalary.ShareDtos;

public record MonthlySalaryResponse
(
    Guid Id,
    string UserId,
    string FullName,
    string Avatar,
    int Month,
    int Year,
    decimal Salary,
    decimal AccountBalance
    );
