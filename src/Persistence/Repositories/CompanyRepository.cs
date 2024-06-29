using Application.Abstractions.Data;
using Application.Abstractions.Shared.Utils;
using Contract.Services.Company.Queries;
using Contract.Services.Company.Shared;
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

            query = query.Where(company => company.NameUnAccent.ToLower().Contains(StringUtils.RemoveDiacritics(request.Name.ToLower())));
        }
        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            query = query.Where(company => company.AddressUnAccent.ToLower().Contains(StringUtils.RemoveDiacritics(request.Address.ToLower())));
        }
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(company => company.Email.ToLower().Contains(request.Email.ToLower()));
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

    public async Task<List<Company>> GetCompanyByNameAsync(string name)
    {
        return await _context.Companies.Where(c => c.NameUnAccent.ToLower().Trim().Contains(name.ToLower().Trim())).ToListAsync();
    }

    public async Task<List<Company>> GetCompanyFactory(CompanyType CompanyType)
    {
        return await _context.Companies.Where(c => c.CompanyType == CompanyType).ToListAsync();
    }

    public async Task<bool> IsCompanyFactoryExistAsync(Guid Id)
    {
        return await _context.Companies
            .AnyAsync(c => c.Id == Id && c.CompanyType == CompanyType.FACTORY);
    }
}