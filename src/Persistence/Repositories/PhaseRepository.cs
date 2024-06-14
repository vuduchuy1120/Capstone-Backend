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
    public async Task<bool> IsExistById(Guid id)
    {
        return await _context.Phases.AnyAsync(x => x.Id == id);
    }
}
