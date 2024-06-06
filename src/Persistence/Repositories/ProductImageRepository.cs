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

    public void DeleteRange(List<ProductImage> productImages)
    {
        _context.ProductImages.RemoveRange(productImages);
    }

    public async Task<List<ProductImage>> GetByProductIdAsync(Guid productId)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .Where(p => p.ProductId.Equals(productId))
            .ToListAsync();
    }

    public async Task<List<ProductImage>> GetProductImageIdsAsync(List<Guid> productImageIds)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .Where(productImage => productImageIds.Contains(productImage.Id))
            .ToListAsync();
    }
}
