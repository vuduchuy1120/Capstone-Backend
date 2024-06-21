using Contract.Services.Company.ShareDto;

namespace Contract.Services.Company.Updates;

public record UpdateCompanyRequest
(
    Guid Id,
    CompanyRequest CompanyRequest
    );