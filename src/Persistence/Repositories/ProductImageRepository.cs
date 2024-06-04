using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

internal class ProductImageRepository : IProductImageRepository
{
    private readonly AppDbContext _context;

    public ProductImageRepository(AppDbContext context)
    {
        _context = context;
    }
    public void Add(ProductImage productImage)
    {
        _context.ProductImages.Add(productImage);
    }

    public void AddRange(List<ProductImage> productImages)
    {
        _context.ProductImages.AddRange(productImages);
    }

    public void Delete(ProductImage productImage)
    {
        _context.ProductImages.Remove(productImage);
    }

    public async Task<ProductImage> GetByIdAsync(Guid id)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id.Equals(id));
    }

    public async Task<List<ProductImage>> GetByProductIdAsync(Guid productId)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .Where(p => p.ProductId.Equals(productId))
            .ToListAsync();
    }

    public void Update(ProductImage productImage)
    {
        _context.ProductImages.Update(productImage);
    }
}
