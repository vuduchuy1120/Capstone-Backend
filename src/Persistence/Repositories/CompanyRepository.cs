using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCompanyExistAsync(Guid companyId)
    {
        return await _context.Companies.AnyAsync(c => c.Id == companyId);
    }
}
