using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class PhaseRepository : IPhaseRepository
{
    private readonly AppDbContext _context;
    public PhaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddPhase(Phase phase)
    {
        _context.Phases.Add(phase);
    }

    public async Task<Phase> GetPhaseById(Guid id)
    {
        return await _context.Phases.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Phase> GetPhaseByName(string name)
    {
        return await _context.Phases.SingleOrDefaultAsync(x => x.Name == name);
    }

    public async Task<List<Phase>> GetPhases()
    {
        return await _context.Phases.ToListAsync();
    }

    public async Task<bool> IsAllPhase1(List<Guid> phaseIds)
    {
        var phaseExistCount = await _context.Phases.CountAsync(x => phaseIds.Contains(x.Id) && x.Name.Equals("PH_001"));
        return phaseExistCount == phaseIds.Count;
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

    public void UpdatePhase(Phase phase)
    {
        _context.Phases.Update(phase);
    }
}
