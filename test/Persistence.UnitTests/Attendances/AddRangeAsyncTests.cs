using Application.Abstractions.Data;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Attendances;

public class AddRangeAttendanceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAttendanceRepository _attendanceRepository;

    public AddRangeAttendanceTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _attendanceRepository = new AttendanceRepository(_context);
    }

    [Fact]
    public async Task AddRangeAttendance_Success_ShouldAddNewAttendancesToDb()
    {
        // Arrange
        var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
            UserId: "001201011091",
            IsAttendance: true,
            HourOverTime: 0.5,
            IsManufacture: true,
            IsSalaryByProduct: false
            );

        var attendance1 = Attendance.Create(createAttendanceRequest1, "01/01/2004", 1, "001201011091");

        var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
            UserId: "034202001936",
            IsAttendance: true,
            HourOverTime: 0.5,
            IsManufacture: true,
            IsSalaryByProduct: false);

        var attendance2 = Attendance.Create(createAttendanceRequest2, "01/01/2004", 5, "034202001936");

        var attendances = new List<Attendance> { attendance1, attendance2 };

        // Act
        await _attendanceRepository.AddRangeAsync(attendances);
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(2, _context.Attendances.CountAsync().Result);
    }

    [Fact]
    public async Task AddRangeAttendance_IdExisted_Error_ShouldNotAddNewAttendancesToDb()
    {
        // Arrange
        var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
            UserId: "001201011091",
            IsAttendance: true,
            HourOverTime: 0.5,
            IsManufacture: true,
            IsSalaryByProduct: false
            );

        var attendance1 = Attendance.Create(createAttendanceRequest1, "01/01/2004", 1, "001201011091");

        var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
            UserId: "034202001936",
            IsAttendance: true,
            HourOverTime: 0.5,
            IsManufacture: true,
            IsSalaryByProduct: false);

        var attendance2 = Attendance.Create(createAttendanceRequest2, "01/01/2001", 5, "034202001936");

        var attendances = new List<Attendance> { attendance1, attendance2 };

        await _attendanceRepository.AddRangeAsync(attendances);
        await _context.SaveChangesAsync();

        var duplicateAttendance1 = Attendance.Create(createAttendanceRequest1, "01/01/2001", 1, "001201011091");
        var duplicateAttendance2 = Attendance.Create(createAttendanceRequest2, "01/01/2001",5, "034202001936");

        var duplicateAttendances = new List<Attendance> { duplicateAttendance1, duplicateAttendance2 };

        // Act and Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _attendanceRepository.AddRangeAsync(duplicateAttendances);
            await _context.SaveChangesAsync();
        });

        Assert.Equal(2, _context.Attendances.CountAsync().Result); // Ensure only original attendances are saved
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
