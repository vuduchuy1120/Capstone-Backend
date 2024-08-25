using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.Logout;
using Contract.Services.User.Logout;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class LogoutCommandHandlerTest
{
    private readonly Mock<IRedisService> _redisServiceMock;
    private readonly Mock<ITokenRepository> _tokenRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    public LogoutCommandHandlerTest()
    {
        _redisServiceMock = new();
        _tokenRepositoryMock = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserIdConflictException_WhenIdLoggedInDiffirentIdLogout()
    {
        var logoutCommand = new LogoutCommand("loggedinID", "logoutId");
        var logoutCommandHandler = new LogoutCommandHandler(
            _redisServiceMock.Object,
            _tokenRepositoryMock.Object,
            _unitOfWorkMock.Object);

        await Assert.ThrowsAsync<UserIdConflictException>(async () =>
        {
            await logoutCommandHandler.Handle(logoutCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_ResultSuccess()
    {
        var logoutCommand = new LogoutCommand("loggedinID", "loggedinID");
        var logoutCommandHandler = new LogoutCommandHandler(
           _redisServiceMock.Object,
           _tokenRepositoryMock.Object,
           _unitOfWorkMock.Object);

        _tokenRepositoryMock.Setup(repo => repo.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(Token.Create("userId", "accessToken", "refreshToken"));

        var result = await logoutCommandHandler.Handle(logoutCommand, default);

        Assert.True(result.isSuccess);
    }
}
