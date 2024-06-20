using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Attendances
{
    public class GetAttendancesByKeysTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IAttendanceRepository _attendanceRepository;

        public GetAttendancesByKeysTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _attendanceRepository = new AttendanceRepository(_context);
        }

        [Fact]
        public async Task GetAttendancesByKeys_ShouldReturnMatchingAttendances()
        {
            // Arrange
            var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "001201011091",
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance1 = Attendance.Create(createAttendanceRequest1, 1, "001201011091");

            var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "034202001936",
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance2 = Attendance.Create(createAttendanceRequest2, 1, "034202001936");

            var attendances = new List<Attendance> { attendance1, attendance2 };

            await _attendanceRepository.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();

            var slotId = 1;
            var date = DateUtil.ConvertStringToDateTimeOnly(DateTime.UtcNow.ToString("dd/MM/yyyy"));
            var userIds = new List<string> { "001201011091", "034202001936" };

            // Act
            var result = await _attendanceRepository.GetAttendancesByKeys(slotId, date, userIds);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.UserId == "001201011091");
            Assert.Contains(result, a => a.UserId == "034202001936");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
