using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class ProductUnitRepository : IProductUnitRepository
{
    private readonly AppDbContext _context;

    public ProductUnitRepository(AppDbContext context)
    {
        _context = context;
    }
    public void Add(ProductUnit productUnit)
    {
        _context.ProductUnits.Add(productUnit);
    }

    public void AddRange(List<ProductUnit> productUnits)
    {
        _context.ProductUnits.AddRange(productUnits);
    }

    public void Delete(ProductUnit productUnit)
    {
        _context.ProductUnits.Remove(productUnit);
    }

    public void DeleteRange(List<ProductUnit> productUnits)
    {
        _context.ProductUnits.RemoveRange(productUnits);
    }

    public async Task<List<ProductUnit>> GetBySubProductIdsAsync(Guid productId, List<Guid> subProductIds)
    {
        return await _context.ProductUnits
            .AsNoTracking()
            .Where(productUnit => productUnit.ProductId.Equals(productId) 
                        && subProductIds.Contains(productUnit.SubProductId))
            .ToListAsync();
    }

    public void Update(ProductUnit productUnit)
    {
        _context.ProductUnits.Update(productUnit);
    }
}
