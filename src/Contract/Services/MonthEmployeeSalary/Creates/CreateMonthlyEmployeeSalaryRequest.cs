namespace Contract.Services.MonthEmployeeSalary.Creates;

public record CreateMonthlyEmployeeSalaryRequest
(
    string UserId,
    int Month,
    int Year,
    decimal Salary
    );
