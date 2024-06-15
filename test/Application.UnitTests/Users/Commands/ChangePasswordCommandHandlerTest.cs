using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.ChangePassword;
using Contract.Services.User.ChangePassword;
using Contract.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class ChangePasswordCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly IValidator<ChangePasswordRequest> _validator;
    public ChangePasswordCommandHandlerTest()
    {
        _passwordServiceMock = new();
        _userRepositoryMock = new();
        _unitOfWorkMock = new();
        _validator = new ChangePasswordValidator();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserIdConflictException_WhenIdNotSame()
    {
        var changePasswordRequest = new ChangePasswordRequest("UserId", "OldPassword", "NewPassword");
        var changePasswordCommand = new ChangePasswordCommand(changePasswordRequest, "loggedInUserId");
        var changePasswordCommandHandler = new ChangePasswordCommandHandler(
            _userRepositoryMock.Object, 
            _unitOfWorkMock.Object, 
            _passwordServiceMock.Object,
            _validator);

        await Assert.ThrowsAsync<UserIdConflictException>(async () =>
        {
            await changePasswordCommandHandler.Handle(changePasswordCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_NewPasswordNotChangeException_WhenNewPasswordSameWithOldPassword()
    {
        var changePasswordRequest = new ChangePasswordRequest("UserId", "OldPassword", "OldPassword");
        var changePasswordCommand = new ChangePasswordCommand(changePasswordRequest, "UserId");
        var changePasswordCommandHandler = new ChangePasswordCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        await Assert.ThrowsAsync<NewPasswordNotChangeException>(async () =>
        {
            await changePasswordCommandHandler.Handle(changePasswordCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExistOrNotActive()
    {
        var changePasswordRequest = new ChangePasswordRequest("UserId", "OldPassword", "NewPassword");
        var changePasswordCommand = new ChangePasswordCommand(changePasswordRequest, "UserId");
        var changePasswordCommandHandler = new ChangePasswordCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await changePasswordCommandHandler.Handle(changePasswordCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_WrongIdOrPasswordException_WhenWrongPassword()
    {
        var changePasswordRequest = new ChangePasswordRequest("UserId", "OldPassword", "NewPassword");
        var changePasswordCommand = new ChangePasswordCommand(changePasswordRequest, "UserId");
        var changePasswordCommandHandler = new ChangePasswordCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _passwordServiceMock.Setup(pass => pass.IsVerify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);   

        await Assert.ThrowsAsync<WrongIdOrPasswordException>(async () =>
        {
            await changePasswordCommandHandler.Handle(changePasswordCommand, default);
        });
    }

    [Theory]
    [InlineData("sd")] // Password must be at least 6 characters long
    [InlineData("")] // Password cannot be empty
    [InlineData("DSFDFSSDFSD")] // Password must contain at least one lowercase letter
    [InlineData("sdfsdfsdfd")] // Password must contain at least one uppercase letter
    [InlineData("SDFSDdsfsdf")] // Password must contain at least one special character
    public async Task Handler_ShouldThrow_MyValidationException_WhenPasswordNotValid(string newPassword)
    {
        var changePasswordRequest = new ChangePasswordRequest("UserId", "OldPassword", newPassword);
        var changePasswordCommand = new ChangePasswordCommand(changePasswordRequest, "UserId");
        var changePasswordCommandHandler = new ChangePasswordCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _passwordServiceMock.Setup(pass => pass.IsVerify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await changePasswordCommandHandler.Handle(changePasswordCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var changePasswordRequest = new ChangePasswordRequest("UserId", "OldPassword", "NewPassword@34324");
        var changePasswordCommand = new ChangePasswordCommand(changePasswordRequest, "UserId");
        var changePasswordCommandHandler = new ChangePasswordCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _validator);

        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _passwordServiceMock.Setup(pass => pass.IsVerify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var result = await changePasswordCommandHandler.Handle(changePasswordCommand, default);

        Assert.True(result.isSuccess);
    }
}
