using Contract.Abstractions.Messages;

namespace Contract.Services.MonthEmployeeSalary.Creates;

public record CreateMonthEmployeeSalaryCommand
(
    int month,
    int year
    ) : ICommand;
