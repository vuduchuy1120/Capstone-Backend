using Application.Abstractions.Data;
using Application.Abstractions.Shared.Utils;
using Contract.Services.Material.Get;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MaterialRepository : IMaterialRepository
{
    private readonly AppDbContext _context;

    public MaterialRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddMaterial(Material material)
    {
        _context.Materials.Add(material);
    }

    public async Task<Material?> GetMaterialByIdAsync(int id)
    {
        return await _context.Materials.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Material?>> GetMaterialsByNameAsync(string name)
    {
        return await _context.Materials.AsNoTracking().Where(m => m.Name.Contains(name)).ToListAsync();
    }

    public Task<List<string>> GetMaterialUnitsAsync()
    {
        return _context.Materials.AsNoTracking().Select(m => m.Unit).Distinct().ToListAsync();
    }

    public async Task<bool> IsMaterialExist(int id)
    {
        return await _context.Materials.AnyAsync(material => material.Id == id);

    }

    public async Task<(List<Material>?, int)> SearchMaterialsAsync(GetMaterialsQuery request)
    {
        var query = _context.Materials.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var removeDiacriticsSearchTerm = StringUtils.RemoveDiacritics(request.SearchTerm).ToLower();

            query = query.Where(m => (m.NameUnaccent).ToLower().Contains(removeDiacriticsSearchTerm));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var materials = await query
            .OrderBy(m => m.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return (materials, totalPages);
    }

    public void UpdateMaterial(Material material)
    {
        _context.Materials.Update(material);
    }
}
