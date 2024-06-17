using Application.Abstractions.Data;
using Application.Abstractions.Shared.Utils;
using Application.Utils;
using Contract.Services.MaterialHistory.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MaterialHistoryRepository : IMaterialHistoryRepository
{
    private readonly AppDbContext _context;
    public MaterialHistoryRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddMaterialHistory(MaterialHistory materialHistory)
    {
        _context.MaterialHistories.Add(materialHistory);
    }

    public async Task<(List<MaterialHistory>?, int)> GetMaterialHistoriesByMaterialNameAndDateAsync(GetMaterialHistoriesByMaterialQuery getMaterialHistories)
    {
        var query = _context.MaterialHistories.Include(mh => mh.Material).AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(getMaterialHistories.SearchTerms))
        {
            var normalizedSearchTerms = StringUtils.RemoveDiacritics(getMaterialHistories.SearchTerms).ToLower();
            query = query.Where(mh => mh.Material.NameUnaccent.Contains(normalizedSearchTerms));
        }

        if (!string.IsNullOrEmpty(getMaterialHistories.StartDateImport))
        {
            var formatedDate = DateUtil.ConvertStringToDateTimeOnly(getMaterialHistories.StartDateImport);
            query = query.Where(mh => mh.ImportDate >= formatedDate);
        }
        if (!string.IsNullOrEmpty(getMaterialHistories.EndDateImport))
        {
            var formatedDate = DateUtil.ConvertStringToDateTimeOnly(getMaterialHistories.EndDateImport);
            query = query.Where(mh => mh.ImportDate <= formatedDate);
        }
        int totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / getMaterialHistories.PageSize);
        var materialHistories = await query
            .OrderByDescending(mh => mh.ImportDate)
            .Skip((getMaterialHistories.PageIndex - 1) * getMaterialHistories.PageSize)
            .Take(getMaterialHistories.PageSize)
            .ToListAsync();
        return (materialHistories, totalPages);
    }

    public async Task<MaterialHistory?> GetMaterialHistoryByIdAsync(Guid id)
    {
        return await _context.MaterialHistories.Include(mh => mh.Material).SingleOrDefaultAsync(mh => mh.Id.Equals(id));
    }

    public Task<bool> IsMaterialHistoryExist(Guid id)
    {
        return _context.MaterialHistories.AnyAsync(mh => mh.Id.Equals(id));
    }

    public void UpdateMaterialHistory(MaterialHistory materialHistory)
    {
        _context.MaterialHistories.Update(materialHistory);
    }
}
