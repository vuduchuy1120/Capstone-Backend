using Application.Abstractions.Data;
using Contract.Services.Company.Shared;
using Contract.Services.ProductPhase.Queries;
using Contract.Services.ProductPhase.ShareDto;
using Domain.Entities;
using Domain.Exceptions.Companies;
using Domain.Exceptions.Phases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

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

    public async Task<List<ProductPhase>> GetProductPhaseByShipmentDetailAsync(List<ShipmentDetail> shipmentDetails, Guid companyId)
    {
        var queryRequests = shipmentDetails
        .Select(s => new { s.ProductId, s.PhaseId })
        .Distinct()
        .ToList();

        var query = _context.ProductPhases.AsQueryable();
        foreach (var request in queryRequests)
        {
            query = query.Where(ph =>
                ph.ProductId == request.ProductId &&
                ph.PhaseId == request.PhaseId &&
                ph.CompanyId == companyId);
        }

        var productPhases = await query.ToListAsync();

        return productPhases;
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

    public async Task<bool> IsAllShipDetailProductValid(List<CheckQuantityInstockEnoughRequest> requests)
    {
        var distinctRequests = requests
        .Select(r => new { r.ProductId, r.PhaseId, r.FromCompanyId })
        .Distinct()
        .ToList();

        var query = _context.ProductPhases.AsQueryable();

        foreach (var request in distinctRequests)
        {
            query = query.Where(ph =>
                ph.ProductId == request.ProductId &&
                ph.PhaseId == request.PhaseId &&
                ph.CompanyId == request.FromCompanyId);
        }

        var filteredProductPhases = await query.ToListAsync();

        return distinctRequests.All(request =>
            filteredProductPhases.Any(ph =>
                ph.ProductId == request.ProductId &&
                ph.PhaseId == request.PhaseId &&
                ph.CompanyId == request.FromCompanyId));

        //var productPhases = await query.ToListAsync();

        //if (productPhases.Count != requests.Count)
        //{
        //    return false;
        //}

        //foreach (var request in requests)
        //{
        //    var productPhase = productPhases.SingleOrDefault(ph
        //        => ph.ProductId == request.ProductId
        //        && ph.PhaseId == request.PhaseId
        //        && ph.CompanyId == request.FromCompanyId);

        //    if (productPhase == null || productPhase.AvailableQuantity < request.Quantity)
        //    {
        //        return false;
        //    }
        //}

        //return true;
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
        _context.ProductPhases.UpdateRange(productPhases);
    }

    public async Task<List<ProductPhase>> GetProductPhaseOfMainFactoryDoneByProductIdsAsync(List<Guid> productIds)
    {
        var phase = await _context.Phases.FirstOrDefaultAsync(p => p.Name == "PH_003")
            ?? throw new PhaseNotFoundException();
        var mainFactory = await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyType == CompanyType.FACTORY && c.Name == "Cơ sở chính")
            ?? throw new CompanyNotFoundException();

        return await _context.ProductPhases
            .Where(ph => productIds.Contains(ph.ProductId) 
                && ph.PhaseId == phase.Id 
                && ph.CompanyId == mainFactory.Id).ToListAsync();
    }

    public async Task<ProductPhase> GetByProductIdPhaseIdAndCompanyIdAsync(Guid productId, Guid phaseId, Guid companyId)
    {
        return await _context.ProductPhases
            .SingleOrDefaultAsync(ph => ph.ProductId == productId && ph.CompanyId == companyId && ph.PhaseId == phaseId);
    }

    public async Task<List<ProductPhase>> GetByProductIdAndCompanyIdAsync(Guid productId, Guid companyId)
    {
        return await _context.ProductPhases
            .Where(ph => ph.ProductId == productId && ph.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<ProductPhase>> GetByProductIdsAndCompanyIdAsync(List<Guid> productIds, Guid companyId)
    {
        return await _context.ProductPhases
            .Where(ph => ph.CompanyId == companyId && productIds.Contains(ph.ProductId))
            .ToListAsync();
    }

}
