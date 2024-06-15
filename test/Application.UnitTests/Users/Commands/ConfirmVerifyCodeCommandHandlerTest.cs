using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.ConfirmVerifyCode;
using Contract.Services.User.ConfirmVerifyCode;
using Contract.Services.User.ForgetPassword;
using Contract.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class ConfirmVerifyCodeCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    private readonly IValidator<ConfirmVerifyCodeCommand> _validator;

    public ConfirmVerifyCodeCommandHandlerTest()
    {
        _passwordServiceMock = new();
        _userRepositoryMock = new();
        _unitOfWorkMock = new();
        _redisServiceMock = new();
        _validator = new ConfirmVerifyCodeValidator();
    }

    [Fact]
    public async Task Handler_ShouldThrow_VerifyCodeNotValidException_WhenNotHaveInfoInRedis()
    {
        var confirmVerifyCodeCommand = new ConfirmVerifyCodeCommand("UserId", "VerifyCode", "Password");
        var confirmVerifyCodeCommandHandler = new ConfirmVerifyCodeCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object, 
            _passwordServiceMock.Object, 
            _redisServiceMock.Object, 
            _validator);

        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync((ForgetPasswordRedis)null);

        await Assert.ThrowsAsync<VerifyCodeNotValidException>(async () =>
        {
            await confirmVerifyCodeCommandHandler.Handle(confirmVerifyCodeCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_VerifyCodeNotValidException_WhenNumberUseMoreThan3()
    {
        var confirmVerifyCodeCommand = new ConfirmVerifyCodeCommand("UserId", "VerifyCode", "Password");
        var confirmVerifyCodeCommandHandler = new ConfirmVerifyCodeCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _redisServiceMock.Object,
            _validator);

        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync(ForgetPasswordRedis.CreateInstacneForTest(4, DateTime.Now.AddMinutes(4), confirmVerifyCodeCommand.VerifyCode));

        await Assert.ThrowsAsync<VerifyCodeNotValidException>(async () =>
        {
            await confirmVerifyCodeCommandHandler.Handle(confirmVerifyCodeCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_VerifyCodeNotValidException_WhenCodeExpire()
    {
        var confirmVerifyCodeCommand = new ConfirmVerifyCodeCommand("UserId", "VerifyCode", "Password");
        var confirmVerifyCodeCommandHandler = new ConfirmVerifyCodeCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _redisServiceMock.Object,
            _validator);

        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync(ForgetPasswordRedis.CreateInstacneForTest(1, DateTime.Now.AddMinutes(-10), confirmVerifyCodeCommand.VerifyCode));

        await Assert.ThrowsAsync<VerifyCodeNotValidException>(async () =>
        {
            await confirmVerifyCodeCommandHandler.Handle(confirmVerifyCodeCommand, default);
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
        var confirmVerifyCodeCommand = new ConfirmVerifyCodeCommand("UserId", "VerifyCode", newPassword);
        var confirmVerifyCodeCommandHandler = new ConfirmVerifyCodeCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _redisServiceMock.Object,
            _validator);

        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync(ForgetPasswordRedis.CreateInstacneForTest(1, DateTime.Now.AddMinutes(5), confirmVerifyCodeCommand.VerifyCode));

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await confirmVerifyCodeCommandHandler.Handle(confirmVerifyCodeCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExist()
    {
        var confirmVerifyCodeCommand = new ConfirmVerifyCodeCommand("UserId", "VerifyCode", "Password#123");
        var confirmVerifyCodeCommandHandler = new ConfirmVerifyCodeCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _redisServiceMock.Object,
            _validator);

        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync(ForgetPasswordRedis.CreateInstacneForTest(1, DateTime.Now.AddMinutes(5), confirmVerifyCodeCommand.VerifyCode));
        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync((User) null);

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await confirmVerifyCodeCommandHandler.Handle(confirmVerifyCodeCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var confirmVerifyCodeCommand = new ConfirmVerifyCodeCommand("UserId", "VerifyCode", "Password#123");
        var confirmVerifyCodeCommandHandler = new ConfirmVerifyCodeCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object,
            _redisServiceMock.Object,
            _validator);

        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync(ForgetPasswordRedis.CreateInstacneForTest(1, DateTime.Now.AddMinutes(5), confirmVerifyCodeCommand.VerifyCode));
        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var result = await confirmVerifyCodeCommandHandler.Handle(confirmVerifyCodeCommand, default);

        Assert.True(result.isSuccess);
    }
}
