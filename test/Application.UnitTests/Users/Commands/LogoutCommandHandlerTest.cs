using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.Logout;
using Contract.Services.User.Logout;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class LogoutCommandHandlerTest
{
    private readonly Mock<IRedisService> _redisServiceMock;
    public LogoutCommandHandlerTest()
    {
        _redisServiceMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserIdConflictException_WhenIdLoggedInDiffirentIdLogout()
    {
        var logoutCommand = new LogoutCommand("loggedinID", "logoutId");
        var logoutCommandHandler = new LogoutCommandHandler(_redisServiceMock.Object);

        await Assert.ThrowsAsync<UserIdConflictException>(async () =>
        {
            await logoutCommandHandler.Handle(logoutCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_ResultSuccess()
    {
        var logoutCommand = new LogoutCommand("loggedinID", "loggedinID");
        var logoutCommandHandler = new LogoutCommandHandler(_redisServiceMock.Object);

        var result = await logoutCommandHandler.Handle(logoutCommand, default);

        Assert.True(result.isSuccess);
    }
}
