using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Attendances
{
    public class IsAllAttendancesExistTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IAttendanceRepository _attendanceRepository;

        public IsAllAttendancesExistTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _attendanceRepository = new AttendanceRepository(_context);
        }

        [Fact]
        public async Task IsAllAttendancesExist_AllAttendancesExist_ShouldReturnTrue()
        {
            // Arrange
            var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "001201011091",
                IsAttendance: true,
                HourOverTime: 0.5,
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance1 = Attendance.Create(createAttendanceRequest1, "01/01/2004", 1, "001201011091");

            var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "034202001936",
                IsAttendance: true,
                HourOverTime: 0.5,
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance2 = Attendance.Create(createAttendanceRequest2, "01/01/2004", 1, "034202001936");

            var attendances = new List<Attendance> { attendance1, attendance2 };

            await _attendanceRepository.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();

            var slotId = 1;
            var date = DateUtil.ConvertStringToDateTimeOnly("01/01/2004");
            var userIds = new List<string> { "001201011091", "034202001936" };

            // Act
            var result = await _attendanceRepository.IsAllAttendancesExist(slotId, date, userIds);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsAllAttendancesExist_NotAllAttendancesExist_ShouldReturnFalse()
        {
            // Arrange
            var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "001201011091",
                IsAttendance: true,
                HourOverTime: 0.5,
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance1 = Attendance.Create(createAttendanceRequest1, "01/01/2004", 1, "001201011091");

            var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "034202001936",
                IsAttendance: true,
                HourOverTime: 0.5,
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance2 = Attendance.Create(createAttendanceRequest2, "01/01/2004", 5, "034202001936");

            var attendances = new List<Attendance> { attendance1 };

            await _attendanceRepository.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();

            var slotId = 1;
            var date = DateUtil.ConvertStringToDateTimeOnly(DateUtils.GetNow().ToString("dd/MM/yyyy"));
            var userIds = new List<string> { "001201011091", "034202001936" };

            // Act
            var result = await _attendanceRepository.IsAllAttendancesExist(slotId, date, userIds);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
