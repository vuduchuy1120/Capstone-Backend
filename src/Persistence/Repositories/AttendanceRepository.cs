using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Query;
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

    public Task<List<Attendance>> GetAttendanceByDate(DateOnly date)
    {
        return _context.Attendances
             .AsNoTracking()
             .Include(u => u.User)
             .Where(a => a.Date == date)
             .ToListAsync();
    }

    public Task<List<Attendance>> GetAttendanceByDateAndSlotId(DateOnly date, int slotId)
    {
        return _context.Attendances
            .AsNoTracking()
            .Where(a => a.Date == date && a.SlotId == slotId)
            .ToListAsync();
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

    public Task<bool> IsCanUpdateAttendance(string userId, int slotId, DateOnly date)
    {
        var attendance = _context.Attendances
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId.Equals(userId) && a.SlotId == (slotId) && a.Date == (date));
        if (attendance == null)
        {
            return Task.FromResult(false);
        }
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        if (attendance.Result.UpdatedDate.HasValue)
        {
            var updatedDateOnly = DateOnly.FromDateTime(attendance.Result.UpdatedDate.Value);
            var differenceInDays = (currentDate.ToDateTime(new TimeOnly(0)) - updatedDateOnly.ToDateTime(new TimeOnly(0))).TotalDays;
            return Task.FromResult(differenceInDays <= 2);
        }
        return Task.FromResult(true);
    }

    public async Task<(List<Attendance>?, int)> SearchAttendancesAsync(GetAttendancesQuery request)
    {
        DateOnly formatedDate;
        if (!string.IsNullOrWhiteSpace(request.Date))
        {
            formatedDate = DateUtil.ConvertStringToDateTimeOnly(request.Date);
        }
        else
        {
            formatedDate = DateOnly.FromDateTime(DateTime.Now);
        }
        var query = _context.Attendances.Include(user => user.User)
            .Where(a => a.Date == formatedDate && a.SlotId == request.SlotId);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(attendance => attendance.UserId.Contains(request.SearchTerm));
        }


        var totalItems = await query.CountAsync();

        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var attendances = await query
            .OrderBy(attendances => attendances.CreatedDate)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (attendances, totalPages);
    }

    public void UpdateAttendance(Attendance attendance)
    {
        _context.Attendances.Update(attendance);
    }
}
