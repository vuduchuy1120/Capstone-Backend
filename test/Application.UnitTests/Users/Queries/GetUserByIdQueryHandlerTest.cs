using Application.Abstractions.Data;
using Application.UserCases.Queries.Users;
using AutoMapper;
using Contract.Services.User.GetUserById;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Queries;

public class GetUserByIdQueryHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    public GetUserByIdQueryHandlerTest()
    {
        _mapperMock = new();
        _userRepositoryMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExist()
    {
        var getUserByIdQuery = new GetUserByIdQuery("UserId");
        var getUserByIdQueryHandler = new GetUserByIdQueryHandler(_userRepositoryMock.Object, _mapperMock.Object);

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await getUserByIdQueryHandler.Handle(getUserByIdQuery, default);
        });
    }

    [Fact]  
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var getUserByIdQuery = new GetUserByIdQuery("UserId");
        var getUserByIdQueryHandler = new GetUserByIdQueryHandler(_userRepositoryMock.Object, _mapperMock.Object);

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _mapperMock.Setup(mapper => mapper.Map<UserResponse>(It.IsAny<User>())).Returns(It.IsAny<UserResponse>());

        var result = await getUserByIdQueryHandler.Handle(getUserByIdQuery, default);

        Assert.True(result.isSuccess);
    }
}
