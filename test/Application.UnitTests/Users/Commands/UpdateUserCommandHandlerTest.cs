using Application.Abstractions.Data;
using Moq;

namespace Application.UnitTests.Users.Commands;

internal class UpdateUserCommandHandlerTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateUserCommandHandlerTest()
    {
        _userRepositoryMock = new();
        _unitOfWorkMock = new();
    }
}
