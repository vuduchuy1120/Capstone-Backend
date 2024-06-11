using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class SetProductRepository : ISetProductRepository
{
    private readonly AppDbContext _context;

    public SetProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddRange(List<SetProduct> setProducts)
    {
        _context.SetProducts.AddRange(setProducts);
    }

    public void DeleteRange(List<SetProduct> setProducts)
    {
        _context.SetProducts.RemoveRange(setProducts);
    }

    public async Task<bool> DoProductIdsExistAsync(List<Guid> productIds, Guid setId)
    {
        var numberIdsExist = await _context.SetProducts
            .CountAsync(sp => sp.SetId == setId && productIds.Contains(sp.ProductId));

        return numberIdsExist == productIds.Count();
    }

    public async Task<List<SetProduct>> GetByProductIdsAndSetId(List<Guid> productIds, Guid setId)
    {
        return await _context.SetProducts
            .AsNoTracking()
            .Where(sp => sp.SetId == setId && productIds.Contains(sp.ProductId))
            .ToListAsync();
    }

    public async Task<bool> IsAnyIdExistAsync(List<Guid> productIds, Guid setId)
    {
        return await _context.SetProducts
            .AnyAsync(sp => sp.SetId == setId && productIds.Contains(sp.ProductId));
    }

    public void UpdateRange(List<SetProduct> setProducts)
    {
        _context.SetProducts.UpdateRange(setProducts);
    }
}
