using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.RefreshToken;
using AutoMapper;
using Contract.Services.User.Login;
using Contract.Services.User.RefreshToken;
using Contract.Services.User.SharedDto;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class RefreshTokenCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    private readonly Mock<ITokenRepository> _tokenRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    public RefreshTokenCommandHandlerTest()
    {
        _userRepositoryMock = new();
        _jwtServiceMock = new();
        _passwordServiceMock = new();
        _mapperMock = new();
        _redisServiceMock = new();
        _tokenRepositoryMock = new();
        _unitOfWorkMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_RefreshTokenNotValidException_WhenUserIdNotFoundInToken()
    {
        var refreshTokenCommand = new RefreshTokenCommand("UserId", "RefreshToken");
        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            _redisServiceMock.Object, 
            _jwtServiceMock.Object,
            _tokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);

        _jwtServiceMock.Setup(jwt => jwt.GetUserIdFromToken(It.IsAny<string>())).Returns((string) null);
        _tokenRepositoryMock.Setup(repo => repo.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Token)null);

        await Assert.ThrowsAsync<RefreshTokenNotValidException>(async () =>
        {
            await refreshTokenCommandHandler.Handle(refreshTokenCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_RefreshTokenNotValidException_WhenUserIdFromTokenDifferentUserId()
    {
        var refreshTokenCommand = new RefreshTokenCommand("UserId", "RefreshToken");
        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            _redisServiceMock.Object,
            _jwtServiceMock.Object,
            _tokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);

        _jwtServiceMock.Setup(jwt => jwt.GetUserIdFromToken(It.IsAny<string>())).Returns("UserIdDifferent");

        await Assert.ThrowsAsync<RefreshTokenNotValidException>(async () =>
        {
            await refreshTokenCommandHandler.Handle(refreshTokenCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_RefreshTokenNotValidException_WhenLoggedInUserInRedisIsNull()
    {
        var refreshTokenCommand = new RefreshTokenCommand("UserId", "RefreshToken");
        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            _redisServiceMock.Object,
            _jwtServiceMock.Object,
            _tokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);

        _jwtServiceMock.Setup(jwt => jwt.GetUserIdFromToken(It.IsAny<string>())).Returns("UserId");
        _redisServiceMock.Setup(redis => redis.GetAsync<LoginResponse>(It.IsAny<string>(), default)).ReturnsAsync((LoginResponse)null);
        _tokenRepositoryMock.Setup(repo => repo.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Token)null);

        await Assert.ThrowsAsync<RefreshTokenNotValidException>(async () =>
        {
            await refreshTokenCommandHandler.Handle(refreshTokenCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_RefreshTokenNotValidException_WhenTokenDifferentTokenInRedis()
    {
        var refreshTokenCommand = new RefreshTokenCommand("UserId", "RefreshToken");
        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            _redisServiceMock.Object,
            _jwtServiceMock.Object,
            _tokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);

        _jwtServiceMock.Setup(jwt => jwt.GetUserIdFromToken(It.IsAny<string>())).Returns("UserId");
        _redisServiceMock.Setup(redis => redis.GetAsync<LoginResponse>(It.IsAny<string>(), default))
            .ReturnsAsync(new LoginResponse(null, "AccessToken", "RefreshTokenDifferent"));
        _tokenRepositoryMock.Setup(repo => repo.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(Token.Create("userId", "AccessToken", "Refreshtoken"));

        await Assert.ThrowsAsync<RefreshTokenNotValidException>(async () =>
        {
            await refreshTokenCommandHandler.Handle(refreshTokenCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExist()
    {
        var refreshTokenCommand = new RefreshTokenCommand("UserId", "RefreshToken");
        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            _redisServiceMock.Object,
            _jwtServiceMock.Object,
            _tokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);

        _jwtServiceMock.Setup(jwt => jwt.GetUserIdFromToken(It.IsAny<string>())).Returns("UserId");
        _redisServiceMock.Setup(redis => redis.GetAsync<LoginResponse>(It.IsAny<string>(), default))
            .ReturnsAsync(new LoginResponse(null, "AccessToken", "RefreshToken"));
        _tokenRepositoryMock.Setup(repo => repo.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(Token.Create("userId", "AccessToken", "RefreshToken"));
        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await refreshTokenCommandHandler.Handle(refreshTokenCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var refreshTokenCommand = new RefreshTokenCommand("UserId", "RefreshToken");
        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            _redisServiceMock.Object,
            _jwtServiceMock.Object,
            _tokenRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);

        _jwtServiceMock.Setup(jwt => jwt.GetUserIdFromToken(It.IsAny<string>())).Returns("UserId");
        _redisServiceMock.Setup(redis => redis.GetAsync<LoginResponse>(It.IsAny<string>(), default))
            .ReturnsAsync(new LoginResponse(null, "AccessToken", "RefreshToken"));
        _userRepositoryMock.Setup(repo => repo.GetUserActiveByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _jwtServiceMock.Setup(jwt => jwt.CreateAccessToken(It.IsAny<User>())).ReturnsAsync("accessToken");
        _jwtServiceMock.Setup(jwt => jwt.CreateRefreshToken(It.IsAny<User>())).ReturnsAsync("refreshToken");
        _mapperMock.Setup(mapper => mapper.Map<UserResponse>(It.IsAny<User>())).Returns(It.IsAny<UserResponse>());
        _tokenRepositoryMock.Setup(repo => repo.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(Token.Create("UserId", "AccessToken", "RefreshToken"));

        var result = await refreshTokenCommandHandler.Handle(refreshTokenCommand, default);

        Assert.True(result.isSuccess);
    }
}
