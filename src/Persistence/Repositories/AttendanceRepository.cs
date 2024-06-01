using Application.Abstractions.Data;
using Application.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _context;

    public AttendanceRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddAttendance(Attendance attendance)
    {
        _context.Attendances.Add(attendance);
    }

    public async Task<Attendance?> GetAttendanceByUserIdSlotIdAndDateAsync(string userId, int slotId, DateOnly date)
    {
        var attendance = await _context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId.Equals(userId) &&
                                        a.SlotId == (slotId) &&
                                        a.Date == (date));
        return attendance;
    }

    public void UpdateAttendance(Attendance attendance)
    {
        _context.Attendances.Update(attendance);
    }
}
