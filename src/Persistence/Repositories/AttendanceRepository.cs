﻿using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Attendance.Query;
using Contract.Services.Attendance.ShareDto;
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

    public async Task AddRangeAsync(List<Attendance> attendances)
    {
        await _context.Attendances.AddRangeAsync(attendances);
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

    public async Task<SearchResponse<List<AttendanceOverallResponse>>> GetAttendanceOverallAsync(DateOnly? startDate, DateOnly? endDate, int pageIndex, int pageSize)
    {
        var query = _context.Attendances.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(a => a.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.Date <= endDate.Value);
        }

        var totalRecords = await query.CountAsync();

        var attendances = await query
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.SlotId)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Include(a => a.Slot)
            .Include(a => a.User)
            .ToListAsync();

        var groupedByDate = attendances
            .GroupBy(a => new { a.Date, a.SlotId })
            .Select(g => new
            {
                Date = g.Key.Date,
                SlotId = g.Key.SlotId,
                AttendanceStats = new AttendanceStatisticResponse(
                    SlotId: g.Key.SlotId,
                    TotalAttendance: g.Count(),
                    TotalManufacture: g.Count(a => a.IsManufacture),
                    TotalSalaryByProduct: g.Count(a => a.IsSalaryByProduct),
                    TotalHourOverTime: g.Sum(a => a.HourOverTime),
                    NumberOfPresent: g.Count(a => a.IsAttendance)
                )
            })
            .GroupBy(x => x.Date)
            .Select(g => new AttendanceOverallResponse(
                g.Key,
                g.Select(x => x.AttendanceStats).ToList()
            ))
            .ToList();

        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        return new SearchResponse<List<AttendanceOverallResponse>>(
            pageIndex,
            totalPages,
            groupedByDate);
    }

    public async Task<List<Attendance>> GetAttendancesByKeys(int slotId, DateOnly date, List<string> userIds)
    {
        var query = await _context.Attendances
            .AsNoTracking()
            .Where(a => a.SlotId == slotId && a.Date == date && userIds.Contains(a.UserId))
            .ToListAsync();
        return query;
    }

    public async Task<bool> IsAllAttendancesExist(int slotID, DateOnly date, List<string> userIds)
    {
        var query = await _context.Attendances.CountAsync(at => at.SlotId == slotID && at.Date == date && userIds.Contains(at.UserId));
        return query == userIds.Count;
    }

    public async Task<bool> IsAllCanUpdateAttendance(List<string> userIds, int slotId, DateOnly date)
    {
        var attendances = await _context.Attendances
            .AsNoTracking()
            .Where(a => a.SlotId == slotId && a.Date == date && userIds.Contains(a.UserId))
            .ToListAsync();

        if (!attendances.Any())
        {
            return false;
        }
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        foreach (var attendance in attendances)
        {
            if (attendance.UpdatedDate.HasValue)
            {
                var updatedDateOnly = DateOnly.FromDateTime(attendance.UpdatedDate.Value);
                var differenceInDays = (currentDate.ToDateTime(new TimeOnly(0)) - updatedDateOnly.ToDateTime(new TimeOnly(0))).TotalDays;
                if (differenceInDays > 2)
                {
                    return false;
                }
            }
        }

        return true;

    }

    public async Task<bool> IsAttendanceAlreadyExisted(List<string> userIds, int slotId, DateOnly date)
    {
        var query = await _context.Attendances.AsNoTracking()
            .CountAsync(at => at.SlotId == slotId && at.Date == date && userIds.Contains(at.UserId));
        return query > 0;
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
        var query = _context.Attendances
            .Include(user => user.User)
                .ThenInclude(emp => emp.EmployeeProducts)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.Images)
            .Include(user => user.User)
            .ThenInclude(emp => emp.EmployeeProducts)
            .ThenInclude(p => p.Phase)
            .Where(a => a.Date == formatedDate && a.SlotId == request.SlotId);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(attendance => (attendance.User.FirstName + ' ' + attendance.User.LastName).Contains(request.SearchTerm));
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

    public void UpdateRange(List<Attendance> attendances)
    {
        _context.Attendances.UpdateRange(attendances);
    }
}
