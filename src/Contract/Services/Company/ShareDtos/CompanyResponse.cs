namespace Contract.Services.Company.ShareDtos;

public record CompanyResponse
(
    Guid Id,
    string Name,
    string Address,
    string DirectorName,
    string DirectorPhone,
    string Email,
    string CompanyType
    );
