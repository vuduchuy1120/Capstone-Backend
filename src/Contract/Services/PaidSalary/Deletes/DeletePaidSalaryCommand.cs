using Contract.Abstractions.Messages;

namespace Contract.Services.PaidSalary.Deletes;

public record DeletePaidSalaryCommand
(
    Guid Id
    ) : ICommand;
