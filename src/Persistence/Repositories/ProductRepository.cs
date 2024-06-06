using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal sealed class ProductRepository: IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
    }

    public async Task<Product> GetProductById(Guid id)
    {
        return await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id.Equals(id));
    }

    public async Task<bool> IsAllSubProductIdsExist(List<Guid> SubProductIds)
    {
        var existingSubProductCount = await _context.Products
        .CountAsync(p => SubProductIds.Contains(p.Id));

        return existingSubProductCount == SubProductIds.Count;
    }

    public async Task<bool> IsHaveGroupInSubProductIds(List<Guid> SubProductIds)
    {
        return await _context.Products
            .AnyAsync(p => SubProductIds.Contains(p.Id) 
                && p.IsGroup == true);
    }

    public async Task<bool> IsProductCodeExist(string code)
    {
        return await _context.Products.AnyAsync(p => p.Code == code);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }
}
