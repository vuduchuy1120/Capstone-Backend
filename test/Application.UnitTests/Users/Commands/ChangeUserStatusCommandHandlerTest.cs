using Application.Abstractions.Data;
using Application.UserCases.Commands.Users.ChangeUserStatus;
using Contract.Services.User.BanUser;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class ChangeUserStatusCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    public ChangeUserStatusCommandHandlerTest()
    {
        _unitOfWorkMock = new();
        _userRepositoryMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExist()
    {
        var changeStatusCommand = new ChangeUserStatusCommand("updateBy", "UserId", true);
        var changeUserStatusCommandHandler = new ChangeUserStatusCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await changeUserStatusCommandHandler.Handle(changeStatusCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var changeStatusCommand = new ChangeUserStatusCommand("updateBy", "UserId", true);
        var changeUserStatusCommandHandler = new ChangeUserStatusCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());

        var result = await changeUserStatusCommandHandler.Handle(changeStatusCommand, default);

        Assert.True(result.isSuccess);
    }
}
