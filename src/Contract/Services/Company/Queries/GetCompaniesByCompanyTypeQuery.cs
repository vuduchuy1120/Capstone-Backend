using Contract.Abstractions.Messages;
using Contract.Services.Company.Shared;
using Contract.Services.Company.ShareDtos;

namespace Contract.Services.Company.Queries;

public record GetCompaniesByCompanyTypeQuery
(
    CompanyType CompanyType
    ) : IQuery<List<CompanyResponse>>;
