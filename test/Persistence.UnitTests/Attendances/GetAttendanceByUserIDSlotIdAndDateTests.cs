﻿using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitTests.Attendances;

public class GetAttendanceByUserIDSlotIdAndDateTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAttendanceRepository _attendanceRepository;

    public GetAttendanceByUserIDSlotIdAndDateTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _attendanceRepository = new AttendanceRepository(_context);
    }
    // handle should return Attendance by UserID, SlotID and Date
    [Fact]
    public async Task GetAttendanceByUserIDSlotIdAndDate_Success_ShouldReturnAttendance()
    {
        await InitDb();
        var fomartedDate = DateUtil.ConvertStringToDateTimeOnly("02/06/2024");
        var retrievedAttendance = await _attendanceRepository.GetAttendanceByUserIdSlotIdAndDateAsync("001201011091", 1, fomartedDate);

        Assert.NotNull(retrievedAttendance);
        Assert.Equal("001201011091", retrievedAttendance.UserId);
        Assert.Equal(1, retrievedAttendance.SlotId);
        Assert.Equal(fomartedDate, retrievedAttendance.Date);
    }
    // handle should return null if not found
    [Fact]
    public async Task GetAttendanceByUserIDSlotIdAndDate_NotFound_ShouldReturnNull()
    {
        await InitDb();
        var fomartedDate = DateUtil.ConvertStringToDateTimeOnly("02/06/2024");
        var retrievedAttendance = await _attendanceRepository.GetAttendanceByUserIdSlotIdAndDateAsync("001201011091", 2, fomartedDate);

        Assert.Null(retrievedAttendance);
    }

    // initDB
    private async Task InitDb()
    {
        var createAttendanceRequest = new CreateAttendanceWithoutSlotIdRequest(
                                   UserId: "001201011091",
                                   HourOverTime: 2,
                                   IsAttendance: true,
                                   IsOverTime: true,
                                   IsSalaryByProduct: false);

        var attendance = Attendance.Create(createAttendanceRequest, 1, "001201011091");
        _attendanceRepository.AddAttendance(attendance);
        await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}