namespace Application.Abstractions.Data;

public interface ICompanyRepository
{
    Task<bool> IsCompanyExistAsync(Guid companyId);
}
