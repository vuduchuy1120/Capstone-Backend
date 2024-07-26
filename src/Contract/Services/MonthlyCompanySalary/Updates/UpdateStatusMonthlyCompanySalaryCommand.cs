
using Contract.Abstractions.Messages;

namespace Contract.Services.MonthlyCompanySalary.Updates;

public record UpdateStatusMonthlyCompanySalaryCommand
(
    UpdateMonthlyCompanySalaryRequest updateReq
    ) : ICommand;
