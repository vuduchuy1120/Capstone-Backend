﻿using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using Contract.Services.Attendance.Update;
using Contract.Services.Role.Create;
using Contract.Services.Slot.Create;
using Contract.Services.User.BanUser;
using Contract.Services.User.CreateUser;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitTests.Attendances;

public class UpdateAttendanceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAttendanceRepository _attendanceRepository;

    public UpdateAttendanceTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _attendanceRepository = new AttendanceRepository(_context);
    }

    [Fact]
    public async Task UpdateAttendance_Success_ShouldUpdateAttendanceInDb()
    {
        var CreateAttendanceDefaultRequest = new CreateAttendanceWithoutSlotIdRequest(
                                                   UserId: "034202001937",
                                                   HourOverTime: 0,
                                                   IsAttendance: true,
                                                   IsOverTime: false,
                                                   IsSalaryByProduct: false);

        var att = Attendance.Create(CreateAttendanceDefaultRequest, 1, "001201011091");
        _attendanceRepository.AddAttendance(att);
        await _context.SaveChangesAsync();

        var updateAttendanceRequest = new UpdateAttendanceWithoutSlotIdRequest(
                    UserId: "034202001937",
                    Date: "02/06/2024",
                    HourOverTime: 2,
                    IsAttendance: true,
                    IsOverTime: true,
                    IsSalaryByProduct: false
                );
        var formatedDate = DateUtil.ConvertStringToDateTimeOnly(updateAttendanceRequest.Date);
        var attendance = await _context.Attendances.FirstOrDefaultAsync(
                       a => a.UserId == updateAttendanceRequest.UserId && a.Date == formatedDate && a.SlotId == 1);

        attendance.Update(updateAttendanceRequest, "001201011091");
        _attendanceRepository.UpdateAttendance(attendance);
        await _context.SaveChangesAsync();

        var savedAttendance = await _context.Attendances.FirstOrDefaultAsync(
                       a => a.UserId == attendance.UserId && a.Date == attendance.Date && a.SlotId == attendance.SlotId);
        Assert.NotNull(savedAttendance);
        Assert.Equal(2, savedAttendance.HourOverTime);
        Assert.True(savedAttendance.IsAttendance);
        Assert.True(savedAttendance.IsOverTime);
        Assert.False(savedAttendance.IsSalaryByProduct);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
