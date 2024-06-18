using Contract.Abstractions.Messages;

namespace Contract.Services.Company.Create;

public record CreateCompanyCommand(
    string Name, 
    string Address,
    string DirectorName,
    string DirectorPhone,
    string Email, 
    string CompanyType) : ICommand;
