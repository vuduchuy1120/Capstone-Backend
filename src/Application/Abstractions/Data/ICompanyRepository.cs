using Domain.Entities;

namespace Application.Abstractions.Data;

public interface ICompanyRepository
{
    Task<bool> IsCompanyExistAsync(Guid companyId);
    Task<Company> GetByIdAsync(Guid id);
}
