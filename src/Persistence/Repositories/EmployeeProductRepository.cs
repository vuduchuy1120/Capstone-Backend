using Application.Abstractions.Data;
using Application.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class EmployeeProductRepository : IEmployeeProductRepository
{
    private readonly AppDbContext _context;
    public EmployeeProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeEmployeeProduct(List<EmployeeProduct> employeeProducts)
    {
        await _context.EmployeeProducts.AddRangeAsync(employeeProducts);
    }

    public void DeleteRangeEmployeeProduct(List<EmployeeProduct> employeeProducts)
    {
        _context.EmployeeProducts.RemoveRange(employeeProducts);
    }

    public async Task<List<EmployeeProduct>> GetEmployeeProductsByDateAndSlotId(int slotId, DateOnly date, Guid companyId)
    {
        return await _context.EmployeeProducts
            .Include(ep => ep.Phase)
            .Include(ep => ep.Product)
            .ThenInclude(p => p.Images)
            .Where(ep => ep.SlotId == slotId && ep.Date == date && ep.User.CompanyId == companyId)
            .ToListAsync();
    }

    public Task<List<EmployeeProduct>> GetEmployeeProductsByEmployeeIdDateAndSlotId(string userId, int slotId, DateOnly date, Guid companyId)
    {
        return _context.EmployeeProducts.Include(ep => ep.Phase).Include(ep => ep.Product).ThenInclude(p => p.Images)
            .Where(ep => ep.UserId == userId && ep.SlotId == slotId && ep.Date == date && ep.User.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<EmployeeProduct>> GetEmployeeProductsByKeysAsync(List<CompositeKey> keys)
    {
        // Convert keys to lists of individual components
        var dates = keys.Select(k => DateUtil.ConvertStringToDateTimeOnly(k.Date)).ToList();
        var slotIds = keys.Select(k => k.SlotId).ToList();
        var productIds = keys.Select(k => k.ProductId).ToList();
        var phaseIds = keys.Select(k => k.PhaseId).ToList();
        var userIds = keys.Select(k => k.UserId).ToList();

        // Query the database for matching records
        var results = await _context.EmployeeProducts
            .Where(ep =>
                dates.Contains(ep.Date) &&
                slotIds.Contains(ep.SlotId) &&
                productIds.Contains(ep.ProductId) &&
                phaseIds.Contains(ep.PhaseId) &&
                userIds.Contains(ep.UserId))
            .ToListAsync();

        return results;
    }

    public async Task<List<EmployeeProduct>> GetEmployeeProductsByMonthAndYearAndUserId(int month, int year, string userId)
    {
        var userAttendances = await _context.Attendances
        .Where(at => at.Date.Month == month &&
                    at.Date.Year == year &&
                    at.UserId == userId &&
                    at.IsAttendance == true &&
                    at.IsSalaryByProduct == true)
        .Select(at => at.Date)
        .ToListAsync();

        var employeeProducts = await _context.EmployeeProducts
            .Include(ep => ep.Product)
                .ThenInclude(p => p.ProductPhaseSalaries)
            .Include(ep => ep.Product)
                .ThenInclude(p => p.Images)
            .Include(ep => ep.Phase)
            .Include(ep => ep.User)
            .Where(ep => ep.Date.Month == month &&
                        ep.Date.Year == year &&
                        ep.UserId == userId &&
                        ep.User.IsActive == true &&
                        userAttendances.Contains(ep.Date))
            .ToListAsync();

        return employeeProducts;
    }

    public async Task<bool> IsAllEmployeeProductExistAsync(List<CompositeKey> keys)
    {
        var keySets = keys.Select(k => new
        {
            Date = DateUtil.ConvertStringToDateTimeOnly(k.Date),
            k.SlotId,
            k.ProductId,
            k.PhaseId,
            k.UserId
        }).ToList();

        int matchingCount = 0;

        foreach (var keySet in keySets)
        {
            var date = keySet.Date;
            var slotId = keySet.SlotId;
            var productId = keySet.ProductId;
            var phaseId = keySet.PhaseId;
            var userId = keySet.UserId;

            var count = await _context.EmployeeProducts
                .Where(ep =>
                    ep.Date == date &&
                    ep.SlotId == slotId &&
                    ep.ProductId == productId &&
                    ep.PhaseId == phaseId &&
                    ep.UserId == userId)
                .CountAsync();

            if (count > 0)
            {
                matchingCount++;
            }
        }

        return matchingCount == keys.Count;
    }
}
