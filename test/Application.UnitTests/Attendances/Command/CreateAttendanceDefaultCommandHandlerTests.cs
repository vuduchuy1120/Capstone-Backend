using Application.Abstractions.Data;
using Application.UserCases.Commands.Attendances.CreateAttendance;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Attendance.Create;
using Domain.Entities;
using FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Attendances.Command;

public class CreateAttendanceDefaultCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;
    private readonly Mock<IValidator<CreateAttendanceDefaultRequest>> _validatorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateAttendanceDefaultCommandHandler _handler;

    public CreateAttendanceDefaultCommandHandlerTests()
    {
        _attendanceRepositoryMock = new Mock<IAttendanceRepository>();
        _validatorMock = new Mock<IValidator<CreateAttendanceDefaultRequest>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new CreateAttendanceDefaultCommandHandler(
            _attendanceRepositoryMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object);
    }
    //[Fact]
    //public async Task Handle_ValidRequest_AddsAttendancesAndSavesChanges()
    //{
    //    // Arrange
    //    var command = new CreateAttendanceDefaultCommand(new CreateAttendanceDefaultRequest
    //    {
    //        CreateAttendances = new List<CreateAttendanceRequest>
    //        {
    //            new CreateAttendanceRequest { /* Initialize properties */ }
    //        },
    //        slotId = 1,
    //        CreatedBy = "TestUser"
    //    });

    //    _validatorMock
    //        .Setup(v => v.ValidateAsync(command.CreateAttendanceDefaultRequest, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(new ValidationResult());

    //    // Act
    //    var result = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    _attendanceRepositoryMock.Verify(r => r.AddAttendance(It.IsAny<Attendance>()), Times.Once);
    //    _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    //    Assert.IsType<Result.Success>(result);
    //}
}
