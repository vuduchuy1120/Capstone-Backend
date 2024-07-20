using Contract.Services.Company.Queries;
using Contract.Services.Company.Shared;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface ICompanyRepository
{
    void Add(Company company);
    void Update(Company company);
    Task<bool> IsExistAsync(Guid id);
    Task<(List<Company>?, int)> SearchCompanyAsync(SearchCompanyQuery request);
    Task<bool> IsCompanyExistAsync(Guid companyId);
    Task<Company> GetByIdAsync(Guid id);
    Task<List<Company>> GetCompanyByNameAsync(string name);
    Task<List<Company>> GetCompanyFactory(CompanyType CompanyType);
    Task<bool> IsCompanyFactoryExistAsync(Guid Id);
    Task<bool> IsCompanyNotCustomerCompanyAsync(Guid CompanyId); 
    Task<bool> IsThirdPartyCompanyAsync(Guid CompanyId);
    Task<List<CompanyType>> GetCompanyTypeByCompanyIdsAsync(List<Guid> companyIds);
    Task<bool> IsCompanyMainFactory(Guid companyId);
}
