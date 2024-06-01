using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.ForgetPassword;
using Contract.Services.User.ForgetPassword;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class ForgetPasswordCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    public ForgetPasswordCommandHandlerTest()
    {
        _redisServiceMock = new();
        _userRepositoryMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExistOrNotActive()
    {
        var forgetPasswordCommand = new ForgetPasswordCommand("UserId");
        var forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(_redisServiceMock.Object, _userRepositoryMock.Object);

        _userRepositoryMock.Setup(repo => repo.IsUserActiveAsync(It.IsAny<string>())).ReturnsAsync(false);

         await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await forgetPasswordCommandHandler.Handle(forgetPasswordCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_ForgetPassworAlreadyRequestException_WhenUserAlreadySendForgetPasswordRequest()
    {
        var forgetPasswordCommand = new ForgetPasswordCommand("UserId");
        var forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(_redisServiceMock.Object, _userRepositoryMock.Object);

        _userRepositoryMock.Setup(repo => repo.IsUserActiveAsync(It.IsAny<string>())).ReturnsAsync(true);
        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync(ForgetPasswordRedis.Create("UserId"));

        await Assert.ThrowsAsync<ForgetPassworAlreadyRequestException>(async () =>
        {
            await forgetPasswordCommandHandler.Handle(forgetPasswordCommand, default);
        });
    }

    [Fact]  
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var forgetPasswordCommand = new ForgetPasswordCommand("UserId");
        var forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(_redisServiceMock.Object, _userRepositoryMock.Object);

        _userRepositoryMock.Setup(repo => repo.IsUserActiveAsync(It.IsAny<string>())).ReturnsAsync(true);
        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync((ForgetPasswordRedis) null);

        var result = await forgetPasswordCommandHandler.Handle(forgetPasswordCommand, default);

        Assert.True(result.isSuccess);
    }
}
