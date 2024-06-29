using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.Login;
using AutoMapper;
using Contract.Services.User.CreateUser;
using Contract.Services.User.Login;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class LoginCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    public LoginCommandHandlerTest()
    {
        _userRepositoryMock = new();
        _jwtServiceMock = new();
        _passwordServiceMock = new();
        _mapperMock = new();
        _redisServiceMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExist()
    {
        var loginCommand = new LoginCommand("userID", "Passsord");
        _userRepositoryMock.Setup(repo => repo.GetUserByPhoneNumberOrIdAsync(loginCommand.Id)).ReturnsAsync((User) null);

        var loginCommandHandler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _passwordServiceMock.Object,
            _mapperMock.Object,
            _redisServiceMock.Object);

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await loginCommandHandler.Handle(loginCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_WrongIdOrPasswordException_WhenPasswordIsWrong()
    {
        var loginCommand = new LoginCommand("userID", "Password");

        _userRepositoryMock.Setup(repo => repo.GetUserByPhoneNumberOrIdAsync(loginCommand.Id)).ReturnsAsync(new User());
        _passwordServiceMock.Setup(service => service.IsVerify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var loginCommandHandler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _passwordServiceMock.Object,
            _mapperMock.Object,
            _redisServiceMock.Object);

        await Assert.ThrowsAsync<WrongIdOrPasswordException>(async () =>
        {
            await loginCommandHandler.Handle(loginCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_ResultSuccess()
    {
        var loginCommand = new LoginCommand("userID", "Password");

        _userRepositoryMock.Setup(repo => repo.GetUserByPhoneNumberOrIdAsync(loginCommand.Id)).ReturnsAsync(new User());
        _passwordServiceMock.Setup(service => service.IsVerify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _jwtServiceMock.Setup(jwt => jwt.CreateAccessToken(It.IsAny<User>())).ReturnsAsync("accessToken");
        _jwtServiceMock.Setup(jwt => jwt.CreateRefreshToken(It.IsAny<User>())).ReturnsAsync("refreshToken");
        _mapperMock.Setup(mapper => mapper.Map<UserResponse>(It.IsAny<User>())).Returns(It.IsAny<UserResponse>());

        var loginCommandHandler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _passwordServiceMock.Object,
            _mapperMock.Object,
            _redisServiceMock.Object);

        var result = await loginCommandHandler.Handle(loginCommand, default);

        Assert.True(result.isSuccess);
    }

}
