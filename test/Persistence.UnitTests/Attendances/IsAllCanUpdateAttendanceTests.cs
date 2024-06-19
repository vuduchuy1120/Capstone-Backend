using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.Attendances
{
    public class IsAllCanUpdateAttendanceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IAttendanceRepository _attendanceRepository;

        public IsAllCanUpdateAttendanceTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _attendanceRepository = new AttendanceRepository(_context);
        }

        [Fact]
        public async Task IsAllCanUpdateAttendance_AllUsersCanUpdate_ShouldReturnTrue()
        {
            var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "001201011091",
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance1 = Attendance.Create(createAttendanceRequest1, 1, "001201011091");

            var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "034202001936",
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance2 = Attendance.Create(createAttendanceRequest2, 5, "034202001936");

            var attendances = new List<Attendance> { attendance1, attendance2 };

            // Act
            await _attendanceRepository.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();

            var userIds = new List<string> { "001201011091", "034202001936" };
            var slotId = 1;
            var date = DateUtil.ConvertStringToDateTimeOnly(DateUtils.GetNow().ToString("dd/MM/yyyy"));

            // Act
            var result = await _attendanceRepository.IsAllCanUpdateAttendance(userIds, slotId, date);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsAllCanUpdateAttendance_NotAllUsersCanUpdate_ShouldReturnFalse()
        {
            // Arrange
            var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "001201011091",
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance1 = Attendance.Create(createAttendanceRequest1, 1, "001201011091");

            var attendances = new List<Attendance> { attendance1 };

            // Add only one attendance
            await _attendanceRepository.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();

            var userIds = new List<string> { "001201011091", "034202001936" };
            var slotId = 1;
            var date = DateUtil.ConvertStringToDateTimeOnly(DateUtils.GetNow().AddDays(2).ToString("dd/MM/yyyy"));

            // Act
            var result = await _attendanceRepository.IsAllCanUpdateAttendance(userIds, slotId, date);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
