using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Application.Users.Create;
using Domain.Users;
using Moq;

namespace Application.UnitTests.Users.Command;

public class CreateUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;

    public CreateUserCommandHandlerTest()
    {
        _userRepositoryMock = new();
        _unitOfWorkMock = new();
        _passwordServiceMock = new();
    }

    [Fact]
    public async Task Handler_Should_Throw_UserAlreadyExistedException()
    {
        var command = new CreateUserCommand()
        {
            Id = "001201011091",
            Fullname = "Nguyen Dinh Son",
            Address = "Ha Noi",
            Password = "password",
            Phone = "0976099351",
            RoleId = 1,
        };

        var handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object, 
            _unitOfWorkMock.Object, 
            _passwordServiceMock.Object);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(command.Id)).ReturnsAsync(true);

        Assert.ThrowsAsync<UserAlreadyExistedException>(async () =>
                await handler.Handle(command, default));
    }

    [Fact]
    public async Task Handler_Show_Return_SuccessResult()
    {
        var command = new CreateUserCommand()
        {
            Id = "001201011091",
            Fullname = "Nguyen Dinh Son",
            Address = "Ha Noi",
            Password = "password",
            Phone = "0976099351",
            RoleId = 1,
        };

        var handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _passwordServiceMock.Object);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(command.Id)).ReturnsAsync(false);
        _passwordServiceMock.Setup(service => service.Hash(command.Password)).Returns("hashed_password");

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Data.Id);
        _userRepositoryMock.Verify(user => user.AddUser(It.Is<User>(u => u.Id == command.Id)), Times.Once);
    }
}
