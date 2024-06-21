using Application.Abstractions.Data;
using Application.Utils;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

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
        var fomartedDate = DateUtil.ConvertStringToDateTimeOnly(DateTime.UtcNow.Date.ToString("dd/MM/yyyy"));
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
        var fomartedDate = DateUtil.ConvertStringToDateTimeOnly(DateTime.UtcNow.Date.ToString("dd/MM/yyyy"));
        var retrievedAttendance = await _attendanceRepository.GetAttendanceByUserIdSlotIdAndDateAsync("001201011091", 2, fomartedDate);

        Assert.Null(retrievedAttendance);
    }

    // initDB
    private async Task InitDb()
    {
        var createAttendanceRequest = new CreateAttendanceWithoutSlotIdRequest(
                                    UserId: "001201011091",
                                    IsAttendance: true,
                                    HourOverTime: 0.5,
                                    IsManufacture: true,
                                    IsSalaryByProduct: false);

        var attendance = Attendance.Create(createAttendanceRequest, "01/01/2004", 1, "001201011091");
        _attendanceRepository.AddAttendance(attendance);
        await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
