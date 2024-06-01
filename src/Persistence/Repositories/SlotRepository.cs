using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class SlotRepository : ISlotRepository
{
    private readonly AppDbContext _context;

    public SlotRepository(AppDbContext context)
    {
        _context = context;
    }

    public void AddSlot(Slot slot)
    {
        _context.Slots.Add(slot);
    }

    public Task<List<Slot>> GetAllSlotsAsync()
    {
        return _context.Slots
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> IsSlotExisted(int slotId)
    {
        return await _context.Slots.AnyAsync(slot => slot.Id == slotId);
    }
}
