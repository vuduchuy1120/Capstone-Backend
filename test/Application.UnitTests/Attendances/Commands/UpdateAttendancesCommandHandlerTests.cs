using Application.Abstractions.Data;
using Application.UserCases.Commands.Attendances.UpdateAttendance;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Update;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Attendances.Command;

public class UpdateAttendancesCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ISlotRepository> _slotRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<UpdateAttendancesRequest> _validator;
    private readonly UpdateAttendancesCommandHandler _handler;

    public UpdateAttendancesCommandHandlerTests()
    {
        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _slotRepositoryMock = new Mock<ISlotRepository>();
        _validator = new UpdateAttendancesRequestValidator(
            _userRepositoryMock.Object,
            _slotRepositoryMock.Object,
            _attendanceRepositoryMock.Object
        );
        _handler = new UpdateAttendancesCommandHandler(
            _attendanceRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator
        );
    }

    [Fact]
    public async Task Handle_Should_Return_SuccessResult()
    {
        // Arrange
        var request = new UpdateAttendancesRequest(
            SlotId: 2,
            UpdateAttendances: new List<UpdateAttendanceWithoutSlotIdRequest>
            {
                new UpdateAttendanceWithoutSlotIdRequest(
                    UserId: "001201011091",
                    Date: "01/06/2024",
                    HourOverTime: 2,
                    IsAttendance: true,
                    IsOverTime: true,
                    IsSalaryByProduct: false
                ),
                new UpdateAttendanceWithoutSlotIdRequest(
                    UserId: "034202001936",
                    Date: "01/06/2024",
                    HourOverTime: 0,
                    IsAttendance: false,
                    IsOverTime: false,
                    IsSalaryByProduct: false
                )
            }
        );

        var command = new UpdateAttendancesCommand(request, "001201011091");

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);
        _attendanceRepositoryMock.Setup(x => x.GetAttendanceByUserIdSlotIdAndDateAsync(
                           It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>())).ReturnsAsync(new Attendance());
        _attendanceRepositoryMock.Setup(x => x.IsCanUpdateAttendance(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>())).ReturnsAsync(true);
        _userRepositoryMock.Setup(x => x.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<Result.Success>(result);
    }

    [Fact]
    public async Task Handle_Should_Throw_MyValidationException_WhenSlotIdNotExist()
    {
        // Arrange
        var request = new UpdateAttendancesRequest(
            SlotId: 2,
            UpdateAttendances: new List<UpdateAttendanceWithoutSlotIdRequest>
            {
                new UpdateAttendanceWithoutSlotIdRequest(
                    UserId: "001201011091",
                    Date: "01/06/2024",
                    HourOverTime: 2,
                    IsAttendance: true,
                    IsOverTime: true,
                    IsSalaryByProduct: false
                ),
                new UpdateAttendanceWithoutSlotIdRequest(
                    UserId: "034202001936",
                    Date: "01/06/2024",
                    HourOverTime: 0,
                    IsAttendance: false,
                    IsOverTime: false,
                    IsSalaryByProduct: false
                )
            }
        );

        var command = new UpdateAttendancesCommand(request, "001201011091");

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        async Task Act() => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MyValidationException>(Act);
    }

    // theory and test cases for all the validation rules
    [Theory]
    [InlineData(1, "001201011091", "01-06-2024", 2, true, true, false, true)] // date is invalid
    [InlineData(2, "001201011091", "01/06/2024", 2, false, true, false, true)] // slotid invalid, isOverTime invalid
    [InlineData(3, "001201011091", "01/06/2024", 2, true, false, false, true)]
    [InlineData(4, "001201011091", "01/06/2024", 2, false, false, true, true)]
    [InlineData(4, "001201011091", "01/06/2024", -5, false, false, true, true)]
    [InlineData(4, "001201011091", "01/06/2024", -5, true, false, true, true)]
    [InlineData(4, "001201011091", "01/06/2024", -5, true, true, true, true)]
    public async Task Handle_Should_Throw_MyValidationException(
                        int slotId,
                        string userId,
                        string date,
                        double hourOverTime,
                        bool isAttendance,
                        bool isOverTime,
                        bool isSalaryByProduct,
                        bool expectException)
    {
        // Arrange
        var request = new UpdateAttendancesRequest(
            SlotId: slotId,
            UpdateAttendances: new List<UpdateAttendanceWithoutSlotIdRequest>
            {
        new UpdateAttendanceWithoutSlotIdRequest(
            UserId: userId,
            Date: date,
            HourOverTime: hourOverTime,
            IsAttendance: isAttendance,
            IsOverTime: isOverTime,
            IsSalaryByProduct: isSalaryByProduct
        )
            }
        );

        var command = new UpdateAttendancesCommand(request, "001201011091");

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);
        //check if user exist
        _userRepositoryMock.Setup(x => x.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);

        //check if attendance exist
        _attendanceRepositoryMock.Setup(x => x.GetAttendanceByUserIdSlotIdAndDateAsync(
                           It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>())).ReturnsAsync(new Attendance());
        // Assert
        if (expectException)
        {
            await Assert.ThrowsAsync<MyValidationException>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });
        }
    }

    // Handle_Should_Throw_MyValidationException_WhenUserIdNotExist()
    [Fact]
    public async Task Handle_ShouldThrow_MyValidationException_WhenUserIdNotExist()
    {
        // Arrange
        var request = new UpdateAttendancesRequest(
            SlotId: 2,
            UpdateAttendances: new List<UpdateAttendanceWithoutSlotIdRequest>
            {
        new UpdateAttendanceWithoutSlotIdRequest(
            UserId: "001201011091",
            Date: "01/06/2024",
            HourOverTime: 2,
            IsAttendance: true,
            IsOverTime: true,
            IsSalaryByProduct: false
        ),
        new UpdateAttendanceWithoutSlotIdRequest(
            UserId: "034202001936",
            Date: "01/06/2024",
            HourOverTime: 0,
            IsAttendance: false,
            IsOverTime: false,
            IsSalaryByProduct: false
        )
            }
        );

        var command = new UpdateAttendancesCommand(request, "001201011091");

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);
        _attendanceRepositoryMock.Setup(x => x.GetAttendanceByUserIdSlotIdAndDateAsync(
            It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>()
        )).ReturnsAsync(new Mock<Attendance>().Object);
        _userRepositoryMock.Setup(x => x.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Act
        async Task Act() => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MyValidationException>(Act);
    }
    // throw MyValidationException when datenow over updatedDate 2 days
    [Fact]
    public async Task Handle_ShouldThrow_MyValidationException_WhenDateNowOverUpdatedDate2Days()
    {
        var request = new UpdateAttendancesRequest(
             SlotId: 2,
             UpdateAttendances: new List<UpdateAttendanceWithoutSlotIdRequest>
             {
                    new UpdateAttendanceWithoutSlotIdRequest(
                        UserId: "001201011091",
                        Date: "01/06/2024",
                        HourOverTime: 2,
                        IsAttendance: true,
                        IsOverTime: true,
                        IsSalaryByProduct: false
                    ),
                    new UpdateAttendanceWithoutSlotIdRequest(
                        UserId: "034202001936",
                        Date: "01/06/2024",
                        HourOverTime: 0,
                        IsAttendance: false,
                        IsOverTime: false,
                        IsSalaryByProduct: false
                    )
             });

        var command = new UpdateAttendancesCommand(request, "001201011091");

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);
        _attendanceRepositoryMock.Setup(x => x.GetAttendanceByUserIdSlotIdAndDateAsync(
                       It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>()
                              )).ReturnsAsync(new Mock<Attendance>().Object);
        _attendanceRepositoryMock.Setup(x => x.IsCanUpdateAttendance(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>())).ReturnsAsync(false);
        _attendanceRepositoryMock.Setup(x => x.IsCanUpdateAttendance(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        async Task Act() => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<MyValidationException>(Act);
    }
}
