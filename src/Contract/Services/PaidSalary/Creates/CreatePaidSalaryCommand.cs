using Contract.Abstractions.Messages;

namespace Contract.Services.PaidSalary.Creates;

public record CreatePaidSalaryCommand
(
    CreatePaidSalaryRequest createReq,
    string CreatedBy
    ) : ICommand;
