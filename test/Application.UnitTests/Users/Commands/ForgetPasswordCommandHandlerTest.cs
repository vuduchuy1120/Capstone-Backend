using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.UserCases.Commands.Users.ForgetPassword;
using Contract.Services.SalaryHistory.Creates;
using Contract.Services.User.CreateUser;
using Contract.Services.User.ForgetPassword;
using Domain.Entities;
using Domain.Exceptions.Users;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class ForgetPasswordCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRedisService> _redisServiceMock;
    private readonly Mock<ISpeedSMSAPI> _speedSMSAPIMock;
    public ForgetPasswordCommandHandlerTest()
    {
        _redisServiceMock = new();
        _userRepositoryMock = new();
        _speedSMSAPIMock = new();
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserIdNotExistOrNotActive()
    {
        var forgetPasswordCommand = new ForgetPasswordCommand("UserId");
        var forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(
            _redisServiceMock.Object, 
            _userRepositoryMock.Object,
            _speedSMSAPIMock.Object);

        _userRepositoryMock.Setup(repo => repo.IsUserActiveAsync(It.IsAny<string>())).ReturnsAsync(false);
        _speedSMSAPIMock.Setup(api => api.sendSMS(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<int>())).Returns("Send success");

         await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await forgetPasswordCommandHandler.Handle(forgetPasswordCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_ForgetPassworAlreadyRequestException_WhenUserAlreadySendForgetPasswordRequest()
    {
        var forgetPasswordCommand = new ForgetPasswordCommand("UserId");
        var forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(
            _redisServiceMock.Object,
            _userRepositoryMock.Object,
            _speedSMSAPIMock.Object);

        _userRepositoryMock.Setup(repo => repo.IsUserActiveAsync(It.IsAny<string>())).ReturnsAsync(true);
        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync(ForgetPasswordRedis.Create("UserId"));
        _speedSMSAPIMock.Setup(api => api.sendSMS(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<int>())).Returns("Send success");

        await Assert.ThrowsAsync<ForgetPassworAlreadyRequestException>(async () =>
        {
            await forgetPasswordCommandHandler.Handle(forgetPasswordCommand, default);
        });
    }

    [Fact]  
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var forgetPasswordCommand = new ForgetPasswordCommand("UserId");
        var forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(
            _redisServiceMock.Object,
            _userRepositoryMock.Object,
            _speedSMSAPIMock.Object);

        var createUserRequest = new CreateUserRequest(
            Id: "001201011091",
            FirstName: "John",
            LastName: "Doe",
            Avatar: "image",
            Phone: "123-456-7890",
            Address: "123 Main St, Anytown, USA",
            Gender: "Male",
            DOB: "10/03/2001",
            SalaryByDayRequest: new SalaryByDayRequest(150, "10/03/2001"),
            SalaryOverTimeRequest: new SalaryOverTimeRequest(200, "10/03/2001"),
            Guid.NewGuid(),
            RoleId: 1
        );
        var user = User.Create(createUserRequest, "SecurePassword123", createUserRequest.Id);

        _userRepositoryMock.Setup(repo => repo.IsUserActiveAsync(It.IsAny<string>())).ReturnsAsync(true);
        _redisServiceMock.Setup(redis => redis.GetAsync<ForgetPasswordRedis>(It.IsAny<string>(), default))
            .ReturnsAsync((ForgetPasswordRedis) null);
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
        _speedSMSAPIMock.Setup(api => api.sendSMS(It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<int>())).Returns("Send success");

        var result = await forgetPasswordCommandHandler.Handle(forgetPasswordCommand, default);

        Assert.True(result.isSuccess);
    }
}
