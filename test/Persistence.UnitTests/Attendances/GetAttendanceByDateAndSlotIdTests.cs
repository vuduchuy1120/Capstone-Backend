using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.Attendances
{
    public class GetAttendanceByDateAndSlotIdTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IAttendanceRepository _attendanceRepository;

        public GetAttendanceByDateAndSlotIdTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _attendanceRepository = new AttendanceRepository(_context);
        }

        [Fact]
        public async Task GetAttendanceByDateAndSlotId_ShouldReturnCorrectAttendances()
        {
            // Arrange
            var date = DateUtil.ConvertStringToDateTimeOnly(DateUtils.GetNow().ToString("dd/MM/yyyy"));
            var slotId = 1;

            var createAttendanceRequest1 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "001201011091",
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance1 = Attendance.Create(createAttendanceRequest1, slotId, "001201011091");

            var createAttendanceRequest2 = new CreateAttendanceWithoutSlotIdRequest(
                UserId: "034202001936",
                IsManufacture: true,
                IsSalaryByProduct: false);

            var attendance2 = Attendance.Create(createAttendanceRequest2, slotId, "034202001936");

            var attendances = new List<Attendance> { attendance1, attendance2 };

            await _attendanceRepository.AddRangeAsync(attendances);
            await _context.SaveChangesAsync();

            // Act
            var result = await _attendanceRepository.GetAttendanceByDateAndSlotId(date, slotId);

            // Assert
            Assert.Equal(2, result.Count);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
