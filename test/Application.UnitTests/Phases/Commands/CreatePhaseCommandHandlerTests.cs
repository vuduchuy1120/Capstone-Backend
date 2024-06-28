using Application.Abstractions.Data;
using Application.UserCases.Commands.Phases.Creates;
using Contract.Services.Phase.Creates;
using Domain.Abstractions.Exceptions;
using Moq;

namespace Application.UnitTests.Phases.Commands;

public class CreatePhaseCommandHandlerTests
{
    private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreatePhaseCommandHandler _handler;

    public CreatePhaseCommandHandlerTests()
    {
        _phaseRepositoryMock = new Mock<IPhaseRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreatePhaseCommandHandler(
                       _phaseRepositoryMock.Object,
                                  _unitOfWorkMock.Object);
    }
    [Fact]
    public async Task Handle_Should_Return_SuccessResult()
    {
        // Arrange
        var request = new CreatePhaseRequest(
                                  Name: "Phase 1",
                                  Description: "Description 1");

        // Act
        var result = await _handler.Handle(new CreatePhaseCommand(request), CancellationToken.None);
        // Assert
        Assert.NotNull(result);
    }

    // should return validation exception
    [Fact]
    public async Task Handle_Should_Throw_ValidationException()
    {
        // Arrange
        var request = new CreatePhaseRequest(
                                Name: "",
                                Description: "Description 1");

        // Act

        // Assert
        Assert.ThrowsAsync<MyValidationException>(
            async () => await _handler.Handle(new CreatePhaseCommand(request), CancellationToken.None));
    }
}
