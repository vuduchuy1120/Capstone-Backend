using Application.Abstractions.Data;
using Application.UserCases.Queries.Users;
using AutoMapper;
using Contract.Services.User.GetUsers;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Queries;

public class GetUsersQueryHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    public GetUsersQueryHandlerTest()
    {
        _mapperMock = new();
        _userRepositoryMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenReceivedUsersIsNull()
    {
        var getUsersQuery = new GetUsersQuery("search", 1, true);
        var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object);

        _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync(((List<User>)null, 0));

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await getUsersQueryHandler.Handle(getUsersQuery, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenReceivedUsersCountEqual0()
    {
        var getUsersQuery = new GetUsersQuery("search", 1, true);
        var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object);

        _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync((new List<User>(), 0));

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await getUsersQueryHandler.Handle(getUsersQuery, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenTotalPageEqual0()
    {
        var getUsersQuery = new GetUsersQuery("search", 1, true);
        var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object);

        _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync((new List<User>() { new User()}, 0));

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await getUsersQueryHandler.Handle(getUsersQuery, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var getUsersQuery = new GetUsersQuery("search", 1, true);
        var getUsersQueryHandler = new GetUsersQueryHandler(_userRepositoryMock.Object, _mapperMock.Object);

        _userRepositoryMock.Setup(repo => repo.SearchUsersAsync(getUsersQuery)).ReturnsAsync((new List<User>() { new User() }, 1));
        _mapperMock.Setup(mapper => mapper.Map<UserResponse>(It.IsAny<User>())).Returns(It.IsAny<UserResponse>());

        var result = await getUsersQueryHandler.Handle(getUsersQuery, default);

        Assert.True(result.isSuccess);
    }
}
