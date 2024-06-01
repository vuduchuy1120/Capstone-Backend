using Application.Abstractions.Data;
using Application.Abstractions.Services;
using AutoMapper;
using Moq;

namespace Application.UnitTests.Users.Commands;

internal class LoginCommandHandlerTest
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
}
