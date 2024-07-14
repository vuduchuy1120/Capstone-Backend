using Application.Abstractions.Data;
using Application.Abstractions.Shared.Utils;
using Contract.Services.Material.Get;
using Contract.Services.Material.Share;
using Domain.Entities;
using MediatR;
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

    public async Task<Material?> GetMaterialByIdAsync(Guid id)
    {
        return await _context.Materials.SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Material>> GetMaterialsByIdsAsync(List<Guid> ids)
    {
        return await _context.Materials
            .Where(m => ids.Contains(m.Id))
            .ToListAsync();
    }

    public async Task<List<Material?>> GetMaterialsByNameAsync(string name)
    {
        return await _context.Materials.AsNoTracking().Where(m => m.Name.Contains(name)).ToListAsync();
    }

    public Task<List<string>> GetMaterialUnitsAsync()
    {
        return _context.Materials.AsNoTracking().Select(m => m.Unit).Distinct().ToListAsync();
    }

    public async Task<bool> IsMaterialEnoughAsync(List<MaterialCheckQuantityRequest> requests)
    {
        var materialIds = requests.Select(r => r.id).ToList();

        var materials = await _context.Materials
            .Where(m => materialIds.Contains(m.Id))
            .ToListAsync();

        if (materials.Count != requests.Count)
        {
            return false;
        }

        foreach (var request in requests)
        {
            var material = materials.FirstOrDefault(m => m.Id == request.id);
            if (material == null || material.QuantityInStock < request.quantity)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> IsMaterialExist(Guid id)
    {
        return await _context.Materials.AnyAsync(material => material.Id == id);

    }

    public async Task<bool> IsMaterialNameExistedAsync(string name)
    {
        var nameUnaccent = StringUtils.RemoveDiacritics(name).ToLower();
        return await _context.Materials.AnyAsync(m => m.NameUnaccent.ToLower().Trim() == nameUnaccent.Trim());
    }

    public async Task<bool> IsUpdateMaterialNameExistedAsync(string name, Guid id)
    {
        var nameUnaccent = StringUtils.RemoveDiacritics(name).ToLower();
        return await _context.Materials.AnyAsync(m => m.NameUnaccent.ToLower().Trim() == nameUnaccent.Trim() && m.Id != id);
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

    public void UpdateRange(List<Material> materials)
    {
        _context.Materials.UpdateRange(materials);
    }
}
