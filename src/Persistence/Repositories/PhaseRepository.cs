using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class PhaseRepository : IPhaseRepository
{
    private readonly AppDbContext _context;
    public PhaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsAllPhaseExistByIdAsync(List<Guid> phaseIds)
    {
        var phaseExistCount = await _context.Phases.CountAsync(x => phaseIds.Contains(x.Id));
        return phaseExistCount == phaseIds.Count;
    }

    public async Task<bool> IsExistById(Guid id)
    {
        return await _context.Phases.AnyAsync(x => x.Id == id);
    }
}
