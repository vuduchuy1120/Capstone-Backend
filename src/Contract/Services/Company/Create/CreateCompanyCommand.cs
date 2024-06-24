using Contract.Abstractions.Messages;
using Contract.Services.Company.Shared;

namespace Contract.Services.Company.Create;

public record CreateCompanyCommand(
    string Name, 
    string Address,
    string DirectorName,
    string DirectorPhone,
    string Email,
    CompanyType CompanyType) : ICommand;
