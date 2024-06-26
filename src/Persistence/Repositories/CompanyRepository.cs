using Application.Abstractions.Data;
using Application.Abstractions.Shared.Utils;
using Contract.Services.Company.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;
public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }
    public void Add(Company company)
    {
        _context.Companies.Add(company);
    }

    public void Update(Company company)
    {
        _context.Companies.Update(company);
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        return await _context.Companies.AnyAsync(company => company.Id.Equals(id));
    }

    public async Task<(List<Company>, int)> SearchCompanyAsync(SearchCompanyQuery request)
    {
        var query = _context.Companies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {

            query = query.Where(company => company.NameUnAccent.Contains(StringUtils.RemoveDiacritics(request.Name)));
        }
        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            query = query.Where(company => company.AddressUnAccent.Contains(StringUtils.RemoveDiacritics(request.Address)));
        }
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(company => company.Email.Contains(request.Email));
        }
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            query = query.Where(company => company.DirectorPhone.Contains(request.PhoneNumber));
        }
        if (request.CompanyType != null)
        {
            query = query.Where(company => company.CompanyType == request.CompanyType);
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var companies = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (companies, totalPages);
    }
    public async Task<bool> IsCompanyExistAsync(Guid companyId)
    {
        return await _context.Companies.AnyAsync(c => c.Id == companyId);
    }
    public async Task<Company> GetByIdAsync(Guid id)
    {
        return await _context.Companies.SingleOrDefaultAsync(c => c.Id == id);
    }

}