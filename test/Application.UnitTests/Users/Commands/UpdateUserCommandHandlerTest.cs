using Application.Abstractions.Data;
using Application.UserCases.Commands.Users.UpdateUser;
using Contract.Services.User.UpdateUser;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Users;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Users.Commands;

public class UpdateUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<UpdateUserRequest> _validator;

    public UpdateUserCommandHandlerTest()
    {
        _userRepositoryMock = new();
        _unitOfWorkMock = new();
        _companyRepositoryMock = new();
        _validator = new UpdateUserValidator(_userRepositoryMock.Object, _companyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenUserIdNotExist()
    {
        var updateUserRequest = new UpdateUserRequest("001201011091", "FirstName", "LastName", 
            "0976099351", "Hanoi", "Male", "10/03/2001", 123, Guid.NewGuid(), 1);
        var updateUserCommand = new UpdateUserCommand(updateUserRequest, "UpdateBy");
        var updateUserCommandHandler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await updateUserCommandHandler.Handle(updateUserCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenFactoryIdNotExist()
    {
        var updateUserRequest = new UpdateUserRequest("001201011091", "FirstName", "LastName",
            "0976099351", "Hanoi", "Male", "10/03/2001", 123, Guid.NewGuid(), 1);
        var updateUserCommand = new UpdateUserCommand(updateUserRequest, "UpdateBy");
        var updateUserCommandHandler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await updateUserCommandHandler.Handle(updateUserCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_MyValidationException_WhenPhoneNumberExist()
    {
        var updateUserRequest = new UpdateUserRequest("001201011091", "FirstName", "LastName",
            "0976099351", "Hanoi", "Male", "10/03/2001", 123, Guid.NewGuid(), 1);
        var updateUserCommand = new UpdateUserCommand(updateUserRequest, "UpdateBy");
        var updateUserCommandHandler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await updateUserCommandHandler.Handle(updateUserCommand, default);
        });
    }

    [Theory]
    [InlineData("001201011091", "John123", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "Male", "10/03/2001", 150, 2)] //firstName contains number or specification character is not valid
    [InlineData("001201011091", "", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "Male", "10/03/2001", 150, 2)] //firstName empty is not valid
    [InlineData("001201011091", "John", "Doe1112", "0976099351", "123 Main St, Anytown, USA",
        "Male", "10/03/2001", 150, 2)] //lastName contains number or specification character is not valid
    [InlineData("001201011091", "John", "", "0976099351", "123 Main St, Anytown, USA",
        "Male", "10/03/2001", 150, 2)] //lastName empty is not valid
    [InlineData("001201011091", "John", "Doe", "0sdfsd976099351", "123 Main St, Anytown, USA",
        "Male", "10/03/2001", 150, 2)] //phone contains characters is not valid
    [InlineData("001201011091", "John", "Doe", "", "123 Main St, Anytown, USA",
        "Male", "10/03/2001", 150, 2)] //phone is empty is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "Male", "10/03/2001", -150, 2)] //salary less than 0 is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "", "10/03/2001", 150, 2)] //gender empty is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "dfd", "10/03/2001", 150, 2)] //gender difference "Male" or "Female" is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "Male", "", 150, 2)] //date empty is not valid
    [InlineData("001201011091", "John", "Doe", "0976099351", "123 Main St, Anytown, USA",
        "Male", "10-03-2001", 150, 2)] //date wrong format dd/MM/yyyy is not valid
    public async Task Handler_Should_Throw_MyValidationException_WhenUserUpdateNotValid(
        string id, string firstName, string lastName, string phone, string address,
        string gender, string dob, int salaryByDay, int roleId)
    {
        var updateUserRequest = new UpdateUserRequest(id, firstName, lastName, phone, address,
            gender, dob, salaryByDay, Guid.NewGuid(), roleId);
        var updateUserCommand = new UpdateUserCommand(updateUserRequest, "UpdateBy");
        var updateUserCommandHandler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object, 
            _unitOfWorkMock.Object, _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await updateUserCommandHandler.Handle(updateUserCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldThrow_UserNotFoundException_WhenUserNotFound()
    {
        var updateUserRequest = new UpdateUserRequest("001201011091", "FirstName", "LastName",
            "0976099351", "Hanoi", "Male", "10/03/2001", 123, Guid.NewGuid(), 1);
        var updateUserCommand = new UpdateUserCommand(updateUserRequest, "UpdateBy");
        var updateUserCommandHandler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((User) null);
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UserNotFoundException>(async () =>
        {
            await updateUserCommandHandler.Handle(updateUserCommand, default);
        });
    }

    [Fact]
    public async Task Handler_ShouldReturn_SuccessResult()
    {
        var updateUserRequest = new UpdateUserRequest("001201011091", "FirstName", "LastName",
            "0976099351", "Hanoi", "Male", "10/03/2001", 123, Guid.NewGuid(), 1);
        var updateUserCommand = new UpdateUserCommand(updateUserRequest, "UpdateBy");
        var updateUserCommandHandler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object, 
            _unitOfWorkMock.Object, _validator);

        _userRepositoryMock.Setup(repo => repo.IsUserExistAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(new User());
        _companyRepositoryMock.Setup(repo => repo.IsCompanyFactoryExistAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(repo => repo.IsPhoneNumberExistAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await updateUserCommandHandler.Handle(updateUserCommand, default);

        Assert.True(result.isSuccess);
    }
}
