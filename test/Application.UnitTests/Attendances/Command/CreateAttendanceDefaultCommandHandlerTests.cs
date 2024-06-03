using Application.Abstractions.Data;
using Application.UserCases.Commands.Attendances.CreateAttendance;
using Contract.Services.Attendance.Create;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Attendances.Command
{
    public class CreateAttendanceDefaultCommandHandlerTests
    {
        private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ISlotRepository> _slotRepositoryMock;

        private readonly IValidator<CreateAttendanceDefaultRequest> _validator;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateAttendanceDefaultCommandHandler _handler;

        public CreateAttendanceDefaultCommandHandlerTests()
        {
            _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _slotRepositoryMock = new Mock<ISlotRepository>();
            _validator = new CreateAttendancesValidator(
                _userRepositoryMock.Object,
                _slotRepositoryMock.Object, 
                _attendanceRepositoryMock.Object);
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CreateAttendanceDefaultCommandHandler(
                _attendanceRepositoryMock.Object,
                _validator,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_SuccessResult()
        {
            // Arrange
            var request = new CreateAttendanceDefaultRequest(
                slotId: 2,
                CreateAttendances: new List<CreateAttendanceWithoutSlotIdRequest>
                {
                    new CreateAttendanceWithoutSlotIdRequest(
                        UserId: "001201011091",
                        HourOverTime: 2,
                        IsAttendance: true,
                        IsOverTime: true,
                        IsSalaryByProduct: false
                    ),
                    new CreateAttendanceWithoutSlotIdRequest(
                        UserId: "034202001936",
                        HourOverTime: 0,
                        IsAttendance: false,
                        IsOverTime: false,
                        IsSalaryByProduct: false
                    )
                });

            var command = new CreateAttendanceDefaultCommand(request, "001201011091");

            _attendanceRepositoryMock.Setup(x => x.GetAttendanceByUserIdSlotIdAndDateAsync(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>()))
                .ReturnsAsync((Attendance?)null);
            _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.isSuccess);
        }

        // write theory and test cases for all the validation rules
        [Theory]
        [InlineData(1, "001201011091", 2, false, true, false, true)] // isOverTime invalid
        [InlineData(-1, "001201011091", 2, false, true, false, true)] // slotid invalid, isOverTime invalid
        [InlineData(1, "001201011091", 0, false, false, false, false)]
        [InlineData(1, "001201011091", 2, false, true, false, true)]
        [InlineData(-1, "001201011091", 2, false, true, false, true)]
        [InlineData(-1, "001201011091", 2, false, true, false, true)]
        public async Task Handle_Should_Throw_MyValidationException(int slotId, string userId, int hourOverTime, bool isAttendance, bool isOverTime, bool isSalaryByProduct, bool expectException)
        {
            // Arrange
            var request = new CreateAttendanceDefaultRequest(
                slotId: slotId,
                CreateAttendances: new List<CreateAttendanceWithoutSlotIdRequest>
                {
                    new CreateAttendanceWithoutSlotIdRequest(
                        UserId: userId,
                        HourOverTime: hourOverTime,
                        IsAttendance: isAttendance,
                        IsOverTime: isOverTime,
                        IsSalaryByProduct: isSalaryByProduct
                    )
                });

            var command = new CreateAttendanceDefaultCommand(request, "001201011091");

            _attendanceRepositoryMock.Setup(x => x.GetAttendanceByUserIdSlotIdAndDateAsync(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>()))
                .ReturnsAsync((Attendance?)null);

            _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(slotId > 0);

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act & Assert
            if (expectException)
            {
                await Assert.ThrowsAsync<MyValidationException>(async () =>
                  await _handler.Handle(command, default));
            }
            else
            {
                var result = await _handler.Handle(command, default);
                Assert.NotNull(result);
                Assert.True(result.isSuccess);
            }
        }
    }
}
