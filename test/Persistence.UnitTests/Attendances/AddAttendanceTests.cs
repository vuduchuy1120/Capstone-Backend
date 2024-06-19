using Application.Abstractions.Data;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Attendances;

public class AddAttendanceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IAttendanceRepository _attendanceRepository;

    public AddAttendanceTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _attendanceRepository = new AttendanceRepository(_context);
    }

    [Fact]
    public async Task AddAttendance_Success_ShouldAddNewAttendanceToDb()
    {
        var createAttendanceRequest = new CreateAttendanceWithoutSlotIdRequest(
                        UserId: "001201011091",
                        IsManufacture : true,
                        IsSalaryByProduct: false);

        var attendance = Attendance.Create(createAttendanceRequest, 1, "001201011091");
        _attendanceRepository.AddAttendance(attendance);
        await _context.SaveChangesAsync();

        Assert.Single(_context.Attendances);
        var savedAttendance = await _context.Attendances.FirstOrDefaultAsync(
            a => a.UserId == attendance.UserId && a.Date == attendance.Date && a.SlotId == attendance.SlotId);
        Assert.NotNull(savedAttendance);
        Assert.Equal("001201011091", savedAttendance.UserId);
    }

    // handle should return SlotID, Date, UserId is existed
    [Fact]
    public async Task AddAttendance_IdExisted_Error_ShouldNotAddNewAttendanceToDb()
    {
        var createAttendanceRequest = new CreateAttendanceWithoutSlotIdRequest(
                        UserId: "034202001936",
                        IsManufacture: true,
                        IsSalaryByProduct: false);
        var attendance = Attendance.Create(createAttendanceRequest, 5, "001201011091");
         _attendanceRepository.AddAttendance(attendance);
        await _context.SaveChangesAsync();

        var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
                        UserId: "034202001936",
                        IsManufacture: true,
                        IsSalaryByProduct: false);
        var attendance2 = Attendance.Create(createAttendanceRequest2, 5, "001201011091");

        // Assert and await the exception
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
             _attendanceRepository.AddAttendance(attendance2);
            await _context.SaveChangesAsync();
        });
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
