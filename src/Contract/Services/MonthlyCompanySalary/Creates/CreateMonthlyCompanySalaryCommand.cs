using Contract.Abstractions.Messages;

namespace Contract.Services.MonthlyCompanySalary.Creates;

public record CreateMonthlyCompanySalaryCommand
(
    int Month,
    int Year
    ) : ICommand;