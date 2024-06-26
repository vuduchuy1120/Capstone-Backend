using Contract.Services.Company.Queries;
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
}
