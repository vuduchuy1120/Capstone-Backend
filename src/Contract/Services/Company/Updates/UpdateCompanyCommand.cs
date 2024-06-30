using Contract.Abstractions.Messages;

namespace Contract.Services.Company.Updates;

public record UpdateCompanyCommand(UpdateCompanyRequest updateCompanyRequest) : ICommand;