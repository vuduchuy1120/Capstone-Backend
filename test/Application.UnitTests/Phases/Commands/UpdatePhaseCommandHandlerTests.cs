using Application.Abstractions.Data;
using Application.UserCases.Commands.Phases.Updates;
using Contract.Services.Material.Create;
using Contract.Services.Phase.Updates;
using Domain.Abstractions.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Phases.Commands;

public class UpdatePhaseCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
    private readonly IValidator<UpdatePhaseRequest> _validator;
    private readonly UpdatePhaseCommandHandler _handler;

    public UpdatePhaseCommandHandlerTests()
    {
        _phaseRepositoryMock = new Mock<IPhaseRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new UpdatePhaseRequestValidator(_phaseRepositoryMock.Object);
        _handler = new UpdatePhaseCommandHandler(
                                  _phaseRepositoryMock.Object,
                                  _unitOfWorkMock.Object,
                                  _validator);
    }
    [Fact]
    public async Task Handle_Should_Return_SuccessResult()
    {
        // Arrange
        var request = new UpdatePhaseRequest(
                            Id: Guid.NewGuid(),
                            Name: "Phase 1",
                            Description: "Description 1");

        var command = new UpdatePhaseCommand(request);

        _phaseRepositoryMock.Setup(x => x.IsExistById(It.IsAny<Guid>())).ReturnsAsync(true);
        _phaseRepositoryMock.Setup(x => x.GetPhaseById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Phase());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    // should return validation exception
    [Fact]
    public async Task Handle_Should_Throw_ValidationException()
    {
        // Arrange
        var request = new UpdatePhaseRequest(
                                       Id: Guid.NewGuid(),
                                                                  Name: "",
                                                                                             Description: "Description 1");

        var command = new UpdatePhaseCommand(request);

        _phaseRepositoryMock.Setup(x => x.IsExistById(It.IsAny<Guid>())).ReturnsAsync(true);
        _phaseRepositoryMock.Setup(x => x.GetPhaseById(It.IsAny<Guid>())).ReturnsAsync(new Domain.Entities.Phase());

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MyValidationException>();
    }
    // notfound id
    [Fact]
    public async Task Handle_Should_Throw_NotFoundException()
    {
        // Arrange
        var request = new UpdatePhaseRequest(
                                            Id: Guid.NewGuid(),
                                            Name: "Phase 1",
                                            Description: "Description 1");

        var command = new UpdatePhaseCommand(request);

        _phaseRepositoryMock.Setup(x => x.IsExistById(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MyValidationException>();
    }
}
