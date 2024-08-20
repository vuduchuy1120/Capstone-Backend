using Application.Abstractions.Data;
using Contract.Services.Product.GetProduct;
using Contract.Services.Product.GetProducts;
using Contract.Services.Product.SearchWithSearchTerm;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Persistence.Repositories;

internal sealed class ProductRepository : IProductRepository
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

    public async Task<Product?> GetProductById(Guid id)
    {
        return await _context.Products
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Images)
            .Include(p => p.ProductPhaseSalaries).ThenInclude(p => p.Phase)
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetProductByIdWithoutImages(Guid id)
    {
        return await _context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetProductByIdWithProductPhase(Guid id)
    {
        return await _context.Products
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Images)
            .Include(p => p.ProductPhaseSalaries).ThenInclude(p => p.Phase)
            .Include(p => p.ProductPhases).ThenInclude(p => p.Phase)
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> IsAllProductIdsExistAsync(List<Guid> productIds)
    {
        var existingProductCount = await _context.Products
            .CountAsync(p => productIds.Contains(p.Id));
        return existingProductCount == productIds.Count;
    }

    public async Task<bool> IsAllProductInProgress(List<Guid> productIds)
    {
        var existingProductCount = await _context.Products
           .CountAsync(p => productIds.Contains(p.Id) && p.IsInProcessing == true);
        return existingProductCount == productIds.Count;
    }

    public async Task<bool> IsAllSubProductIdsExist(List<Guid> SubProductIds)
    {
        var existingSubProductCount = await _context.Products
        .CountAsync(p => SubProductIds.Contains(p.Id));

        return existingSubProductCount == SubProductIds.Count;
    }

    public async Task<bool> IsProductCodeExist(string code)
    {
        return await _context.Products.AnyAsync(p => p.Code == code);
    }

    public async Task<bool> IsProductIdExist(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<(List<Product>?, int)> SearchProductAsync(GetProductsQuery getProductsQuery)
    {
        var query = _context.Products.Where(p => p.IsInProcessing == getProductsQuery.IsInProcessing);

        var searchTerm = getProductsQuery.SearchTerm;
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower())
            || p.Code.ToLower().Contains(searchTerm.ToLower()));
        }

        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / getProductsQuery.PageSize);

        var products = await query
            .OrderBy(p => p.CreatedDate)
            .Skip((getProductsQuery.PageIndex - 1) * getProductsQuery.PageSize)
            .Take(getProductsQuery.PageSize)
            .Include(p => p.Images)
            .AsNoTracking()
            .AsSingleQuery()
            .ToListAsync();

        return (products, totalPages);
    }

    public async Task<(List<Product>, int)> SearchProductAsync(GetWithSearchTermQuery request)
    {
        var query = _context.Products
            .Include(p => p.Images) 
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTermLower = request.SearchTerm.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchTermLower)
                || p.Code.ToLower().Contains(searchTermLower));
        }

        var totalItems = await query.CountAsync();

        int totalPages = request.PageSize > 0 ? (int)Math.Ceiling((double)totalItems / request.PageSize) : 0;

        var pageIndex = Math.Max(request.PageIndex, 1);

        var products = await query
            .Skip((pageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsSplitQuery()
            .ToListAsync();

        return (products, totalPages);
    }


    public void Update(Product product)
    {
        _context.Products.Update(product);
    }
}
