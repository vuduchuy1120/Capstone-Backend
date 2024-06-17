using Application.Abstractions.Data;
using Application.UserCases.Commands.Attendances.CreateAttendance;
using Contract.Services.Attendance.Create;
using Domain.Abstractions.Exceptions;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Attendances.Command;

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
                    IsManufacture: true,
                    IsSalaryByProduct: false
                ),
                new CreateAttendanceWithoutSlotIdRequest(
                    UserId: "034202001936",
                    IsManufacture: true,
                    IsSalaryByProduct: false
                )
            });


        var command = new CreateAttendanceDefaultCommand(request, "001201011091");
        _userRepositoryMock.Setup(x => x.IsAllUserActiveAsync(It.IsAny<List<string>>())).ReturnsAsync(true);

        _attendanceRepositoryMock
        .Setup(x => x.IsAttendanceAlreadyExisted
            (It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<DateOnly>()))
        .ReturnsAsync(false);

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);


        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.isSuccess);
    }



    [Fact]
    //should throw exception if slotId is invalid
    public async Task Handle_Should_Throw_MyValidationException_If_SlotId_Is_Invalid()
    {
        // Arrange
        var request = new CreateAttendanceDefaultRequest(
                        slotId: -1,
                        CreateAttendances: new List<CreateAttendanceWithoutSlotIdRequest>
                        {
                            new CreateAttendanceWithoutSlotIdRequest(
                                UserId: "001201011091",
                                IsManufacture: true,
                                IsSalaryByProduct: false
                            )
                        });

        var command = new CreateAttendanceDefaultCommand(request, "001201011091");

        _userRepositoryMock.Setup(x => x.IsAllUserActiveAsync(It.IsAny<List<string>>())).ReturnsAsync(true);

        _attendanceRepositoryMock
        .Setup(x => x.IsAttendanceAlreadyExisted
                   (It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<DateOnly>()))
        .ReturnsAsync(false);

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
                   await _handler.Handle(command, default));
    }

    [Fact]
    //should throw exception if userId is invalid
    public async Task Handle_Should_Throw_MyValidationException_If_UserId_Is_Invalid()
    {
        // Arrange
        var request = new CreateAttendanceDefaultRequest(
                                   slotId: 1,
                                                          CreateAttendances: new List<CreateAttendanceWithoutSlotIdRequest>
                                                          {
                            new CreateAttendanceWithoutSlotIdRequest(
                                                               UserId: "",
                                                                                              IsManufacture: true,
                                                                                                                             IsSalaryByProduct: false
                                                                                                                                                        )
                        });

        var command = new CreateAttendanceDefaultCommand(request, "001201011091");

        _userRepositoryMock.Setup(x => x.IsAllUserActiveAsync(It.IsAny<List<string>>())).ReturnsAsync(false);

        _attendanceRepositoryMock
        .Setup(x => x.IsAttendanceAlreadyExisted
                          (It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<DateOnly>()))
        .ReturnsAsync(false);

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
                          await _handler.Handle(command, default));
    }
    [Fact]
    //should throw exception if attendance is already existed
    public async Task Handle_Should_Throw_MyValidationException_If_Attendance_Is_Already_Existed()
    {
        // Arrange
        var request = new CreateAttendanceDefaultRequest(
                                              slotId: 1,
                                                                                                       CreateAttendances: new List<CreateAttendanceWithoutSlotIdRequest>
                                                                                                       {
                            new CreateAttendanceWithoutSlotIdRequest(
                                                                                              UserId: "001201011091",
                                                                                                                                                                                           IsManufacture: true,
                                                                                                                                                                                                                                                                                                                       IsSalaryByProduct: false
                                                                                                                                                                                                                                                                                                                                                                                                                                                                              )
                        });

        var command = new CreateAttendanceDefaultCommand(request, "001201011091");

        _userRepositoryMock.Setup(x => x.IsAllUserActiveAsync(It.IsAny<List<string>>())).ReturnsAsync(true);

        _attendanceRepositoryMock
        .Setup(x => x.IsAttendanceAlreadyExisted
                                 (It.IsAny<List<string>>(), It.IsAny<int>(), It.IsAny<DateOnly>()))
        .ReturnsAsync(true);

        _slotRepositoryMock.Setup(x => x.IsSlotExisted(It.IsAny<int>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
                                 await _handler.Handle(command, default));
    }
}
