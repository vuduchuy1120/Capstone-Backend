using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.UnitTests.Attendances;

public class IsCanUpdateAttendance : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAttendanceRepository _attendanceRepository;

    public IsCanUpdateAttendance()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _attendanceRepository = new AttendanceRepository(_context);
    }

    private void InitDatabase()
    {
        var createAttendanceRequest = new CreateAttendanceWithoutSlotIdRequest(
                                              UserId: "001201011091",
                                              IsManufacture: true,
                                              IsSalaryByProduct: false);
        var req = Domain.Entities.Attendance.Create(createAttendanceRequest, 1, "001201011091");
        _attendanceRepository.AddAttendance(req);
        _context.SaveChanges();

        var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
                                                UserId: "034202001936",
                                                IsManufacture: true,
                                                IsSalaryByProduct: false);
        var req2 = Domain.Entities.Attendance.Create(createAttendanceRequest2, 1, "001201011091");
        _attendanceRepository.AddAttendance(req2);
        _context.SaveChanges();
        var fomartedDate = DateUtil.ConvertStringToDateTimeOnly(DateTime.UtcNow.Date.ToString("dd/MM/yyyy"));

        var attendance = _context.Attendances.FirstOrDefault(a => a.Date == fomartedDate && a.SlotId == 1 && a.UserId == "001201011091");
        attendance.UpdatedDate = DateTime.Parse("2024-05-29 03:31:50.259009+00");
        attendance.UpdatedBy = "001201011091";
        _attendanceRepository.UpdateAttendance(attendance);
        _context.SaveChanges();
    }
    // handle should return true if attendance can be update
    [Fact]
    public async Task IsCanUpdateAttendance_Success_ShouldReturnFalse()
    {
        InitDatabase();
        var fomartedDate = DateUtil.ConvertStringToDateTimeOnly(DateTime.UtcNow.Date.ToString("dd/MM/yyyy"));
        var canUpdate = await _attendanceRepository.IsCanUpdateAttendance("001201011091", 1, fomartedDate);
        Assert.False(canUpdate);
    }

    [Fact]
    public async Task IsCanUpdateAttendance_Success_ShouldReturnTrue()
    {
        InitDatabase();
        var fomartedDate = DateUtil.ConvertStringToDateTimeOnly(DateTime.UtcNow.Date.ToString("dd/MM/yyyy"));
        var canUpdate = await _attendanceRepository.IsCanUpdateAttendance("034202001936", 1, fomartedDate);
        Assert.True(canUpdate);
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}
