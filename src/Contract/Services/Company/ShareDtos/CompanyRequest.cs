namespace Contract.Services.Company.ShareDto;

public record CompanyRequest
(
    string Name,
    string Address,
    string DirectorName,
    string DirectorPhone,
    string Email,
    string CompanyType
    );