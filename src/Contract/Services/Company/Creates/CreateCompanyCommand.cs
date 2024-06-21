using Contract.Abstractions.Messages;

namespace Contract.Services.Company.Create;

public record CreateCompanyCommand(CreateCompanyRequest CreateCompanyRequest) : ICommand;
