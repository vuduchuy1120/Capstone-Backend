using Contract.Abstractions.Messages;

namespace Contract.Services.SalaryHistory.Creates;

public record CreateSalaryHistoryCommand
(CreateSalaryHistoryRequest CreateSalaryHistoryRequest) : ICommand;
