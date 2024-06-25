namespace Contract.Services.Company.Shared;

public record CompanyResponse(
    Guid Id, 
    string Name, 
    string Address, 
    string DirectorName, 
    string DirectorPhone,
    string Email);
