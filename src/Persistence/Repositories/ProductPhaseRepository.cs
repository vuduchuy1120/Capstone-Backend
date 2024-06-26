using Application.Abstractions.Data;
using Contract.Services.ProductPhase.Queries;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class ProductPhaseRepository : IProductPhaseRepository
{
    private readonly AppDbContext _context;
    public ProductPhaseRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddProductPhase(ProductPhase productPhase)
    {
        _context.ProductPhases.Add(productPhase);
    }

    public void AddProductPhaseRange(List<ProductPhase> productPhases)
    {
        _context.ProductPhases.AddRange(productPhases);
    }

    public void DeleteProductPhase(ProductPhase productPhase)
    {
        _context.ProductPhases.Remove(productPhase);
    }

    public async Task<ProductPhase> GetByProductIdPhaseIdCompanyID(Guid productId, Guid phaseId, Guid mainCompanyID)
    {
        return await _context.ProductPhases
            .Include(pp => pp.Product)
            .Include(pp => pp.Phase)
            .FirstOrDefaultAsync(pp => pp.ProductId == productId && pp.PhaseId == phaseId && pp.CompanyId == mainCompanyID);
    }

    public async Task<ProductPhase> GetProductPhaseByPhaseIdAndProductId(Guid productId, Guid phaseId)
    {
        return await _context.ProductPhases.SingleOrDefaultAsync(pp => pp.ProductId == productId && pp.PhaseId == phaseId);
    }

    public async Task<(List<ProductPhase>?, int)> GetProductPhases(GetProductPhasesQuery request)
    {
        var query = _context.ProductPhases.Include(pp => pp.Phase).Include(pp => pp.Product).AsNoTracking().AsQueryable();
        var totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);
        var productphases = await query
           .Skip((request.PageIndex - 1) * request.PageSize)
           .Take(request.PageSize)
           .AsNoTracking()
           .ToListAsync();

        return (productphases, totalPages);
    }

    public async Task<List<ProductPhase>> GetProductPhasesByPhaseId(Guid phaseId)
    {
        return await _context.ProductPhases.Where(pp => pp.PhaseId == phaseId).ToListAsync();
    }

    public async Task<List<ProductPhase>> GetProductPhasesByProductId(Guid productId)
    {
        return await _context.ProductPhases.Where(pp => pp.ProductId == productId).ToListAsync();
    }

    public async Task<bool> IsProductPhaseExist(Guid productId, Guid phaseId)
    {
        return await _context.ProductPhases.AnyAsync(pp => pp.ProductId == productId && pp.PhaseId == phaseId);
    }

    public void UpdateProductPhase(ProductPhase productPhase)
    {
        _context.ProductPhases.Update(productPhase);
    }

    public void UpdateProductPhaseRange(List<ProductPhase> productPhases)
    {
        throw new NotImplementedException();
    }
}
