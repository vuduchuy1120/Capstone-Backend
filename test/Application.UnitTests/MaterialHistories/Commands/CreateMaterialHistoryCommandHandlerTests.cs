using Application.Abstractions.Data;
using Application.UserCases.Commands.MaterialHistories.Create;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Create;
using Domain.Abstractions.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Application.UnitTests.MaterialHistories.Commands;

public class CreateMaterialHistoryCommandHandlerTests
{
    private readonly Mock<IMaterialHistoryRepository> _materialHistoryRepositoryMock;
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<CreateMaterialHistoryRequest> _validator;
    private readonly CreateMaterialHistoryCommandHandler _handler;

    public CreateMaterialHistoryCommandHandlerTests()
    {
        _materialHistoryRepositoryMock = new Mock<IMaterialHistoryRepository>();
        _materialRepositoryMock = new Mock<IMaterialRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new CreateMaterialHistoryRequestValidator(
            _materialHistoryRepositoryMock.Object,
            _materialRepositoryMock.Object);
        _handler = new CreateMaterialHistoryCommandHandler(
                                    _materialHistoryRepositoryMock.Object,
                                    _unitOfWorkMock.Object,
                                    _validator);
    }

    [Fact]
    public async Task Handle_Should_Return_SuccessResult()
    {
        // Arrange
        var request = new CreateMaterialHistoryRequest(
                        MaterialId: 1,
                        Quantity: 1,
                        Price: 50000,
                        Description: "Description",
                        ImportDate: "03/06/2024");
        // Act
        _materialRepositoryMock.Setup(x => x.IsMaterialExist(It.IsAny<int>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new CreateMaterialHistoryCommand(request), default);

        // Assert
        result.Should().BeOfType<Result.Success>();
    }

    [Theory]
    [InlineData(1, 0, 50000, "Description", "03/06/2024")] // error quantity
    [InlineData(1, -1, 50000, "Description", "03/06/2024")]
    [InlineData(1, 1, 50000, "Description", "")]
    public async Task Handle_Should_Throw_ValidationException(
        int materialId,
        int quantity,
        int price,
        string description,
        string importDate)
    {
        // Arrange
        var request = new CreateMaterialHistoryRequest(
                            MaterialId: materialId,
                            Quantity: quantity,
                            Price: price,
                            Description: description,
                            ImportDate: importDate);

        _materialRepositoryMock.Setup(x => x.IsMaterialExist(It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(new CreateMaterialHistoryCommand(request), default);

        // Assert
        await act.Should().ThrowAsync<MyValidationException>();
    }

}
