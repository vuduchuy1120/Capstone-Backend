using Application.Abstractions.Data;
using Contract.Services.Product.GetProducts;
using Contract.Services.Set.GetSets;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class SetRepository : ISetRepository
{
    private readonly AppDbContext _context;

    public SetRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Set set)
    {
        _context.Sets.Add(set);
    }

    public async Task<Set?> GetByIdAsync(Guid id)
    {
        return await _context.Sets
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.SetProducts)
                .ThenInclude(sp => sp.Product)
                .ThenInclude(p => p.Images)
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Set?> GetByIdWithoutSetProductAsync(Guid id)
    {
        return await _context.Sets
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> IsAllSetIdExistAsync(List<Guid> setIds)
    {
        if (setIds.Count == 0)
        {
            return false;
        }
        var query = await _context.Sets.Where(s => setIds.Contains(s.Id)).ToListAsync();
        return query.Count == setIds.Count;
    }

    public async Task<bool> IsAllSetExistAsync(List<Guid> ids)
    {
        var numberExist = await _context.Sets.CountAsync(s => ids.Contains(s.Id));
        return numberExist == ids.Count;
    }
    public async Task<bool> IsCodeExistAsync(string code)
    {
        return await _context.Sets.AnyAsync(s => s.Code == code);
    }

    public async Task<(List<Set>, int)> SearchSetAsync(GetSetsQuery request)
    {
        var query = _context.Sets.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s => s.Name.ToLower().Contains(request.SearchTerm.ToLower()) || s.Code.ToLower().Contains(request.SearchTerm.ToLower()));
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var sets = await query
            .OrderBy(p => p.CreatedDate)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return (sets, totalPages);
    }

    public void Update(Set set)
    {
        _context.Sets.Update(set);
    }

    public async Task<List<Set>> SearchSetAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return null;
        }

        return await _context.Sets
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.SetProducts)
                .ThenInclude(sp => sp.Product)
                .ThenInclude(p => p.Images)
            .Where(s => s.Name.ToLower().Contains(searchTerm.ToLower()) || s.Code.ToLower().Contains(searchTerm.ToLower()))
            .ToListAsync();
    }
}
