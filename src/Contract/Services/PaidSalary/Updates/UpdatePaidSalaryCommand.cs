using Contract.Abstractions.Messages;

namespace Contract.Services.PaidSalary.Updates;

public record UpdatePaidSalaryCommand
(
    UpdatePaidSalaryRequest updateReq,
    string UpdatedBy
    ) : ICommand;