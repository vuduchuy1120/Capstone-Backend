using Application.Abstractions.Data;
using Application.UserCases.Commands.MaterialHistories.Update;
using Contract.Abstractions.Shared.Results;
using Contract.Services.MaterialHistory.Update;
using Domain.Abstractions.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;


namespace Application.UnitTests.MaterialHistories.Commands;

public class UpdateMaterialHistoryCommandHandlerTests
{
    private readonly Mock<IMaterialHistoryRepository> _materialHistoryRepositoryMock;
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<UpdateMaterialHistoryRequest> _validator;
    private readonly UpdateMaterialHistoryCommandHandler _handler;

    public UpdateMaterialHistoryCommandHandlerTests()
    {
        _materialHistoryRepositoryMock = new Mock<IMaterialHistoryRepository>();
        _materialRepositoryMock = new Mock<IMaterialRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new UpdateMaterialHistoryRequestValidator(
            _materialHistoryRepositoryMock.Object, _materialRepositoryMock.Object);
        _handler = new UpdateMaterialHistoryCommandHandler(
                        _materialHistoryRepositoryMock.Object,
                        _unitOfWorkMock.Object,
                        _validator);
    }

    [Fact]
    public async Task Handle_Should_Return_SuccessResult()
    {
        // Arrange
        var request = new UpdateMaterialHistoryRequest(
                                Id: new Guid("05a76796-e8ac-4a67-8633-950d00de864e"),
                                MaterialId: Guid.NewGuid(),
                                Quantity: 1,
                                Price: 50000,
                                Description: "Description",
                                ImportDate: "03/06/2024");
        // Act
        _materialHistoryRepositoryMock.Setup(x => x.IsMaterialHistoryExist(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _materialHistoryRepositoryMock.Setup(x => x.GetMaterialHistoryByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.MaterialHistory());

        _materialRepositoryMock.Setup(x => x.IsMaterialExist(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new UpdateMaterialHistoryCommand(request), default);

        // Assert
        result.Should().BeOfType<Result.Success>();
    }
    // should throw notfound exception if material history not found
    [Fact]
    public async Task Handle_Should_Throw_MyValidationException_WhenReceivedMaterialHistoryIsNull()
    {
        // Arrange
        var request = new UpdateMaterialHistoryRequest(
                            Id: new Guid("05a76796-e8ac-4a67-8633-950d00de864e"),
                            MaterialId: Guid.NewGuid(),
                            Quantity: 1,
                            Price: 50000,
                            Description: "Description",
                            ImportDate: "03/06/2024");

        _materialHistoryRepositoryMock.Setup(x => x.IsMaterialHistoryExist(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(new UpdateMaterialHistoryCommand(request), default);
        });
    }
    [Fact]
    public async Task Handle_Should_Throw_MyValidationException_WhenReceivedMaterialIsNull()
    {
        // Arrange
        var request = new UpdateMaterialHistoryRequest(
                            Id: new Guid("05a76796-e8ac-4a67-8633-950d00de864e"),
                            MaterialId: Guid.NewGuid(),
                            Quantity: 1,
                            Price: 50000,
                            Description: "Description",
                            ImportDate: "03/06/2024");

        _materialHistoryRepositoryMock.Setup(x => x.IsMaterialHistoryExist(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _materialHistoryRepositoryMock.Setup(x => x.GetMaterialHistoryByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.MaterialHistory());

        _materialRepositoryMock.Setup(x => x.IsMaterialExist(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            await _handler.Handle(new UpdateMaterialHistoryCommand(request), default);
        });
    }


    [Theory]
    [InlineData("05a76796-e8ac-4a67-8633-950d00de864e", "05a76796-e8ac-4a67-8633-950d00de864e", 0.0, 50000.0, "Description", "03/06/2024")] // error quantity
    [InlineData("05a76796-e8ac-4a67-8633-950d00de864e", "05a76796-e8ac-4a67-8633-950d00de864e", -1.0, 50000.0, "Description", "03/06/2024")]
    [InlineData("05a76796-e8ac-4a67-8633-950d00de864e", "05a76796-e8ac-4a67-8633-950d00de864e", 1.0, 0.0, "Description", "03/06/2024")] // error price
    [InlineData("05a76796-e8ac-4a67-8633-950d00de864e", "05a76796-e8ac-4a67-8633-950d00de864e", 1.0, 50000.0, "Description", "")] // error importDate
    public async Task Handle_Should_Throw_ValidationException(
        string id,
        string materialId,
        double quantity,
        decimal price,
        string? description,
        string importDate)
    {
        // Arrange
        var request = new UpdateMaterialHistoryRequest(
            Id: new Guid(id),
            MaterialId: new Guid(materialId),
            Quantity: quantity,
            Price: price,
            Description: description,
            ImportDate: importDate);

        // error id inline 1 not found
        _materialHistoryRepositoryMock.Setup(x => x.IsMaterialHistoryExist(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _materialHistoryRepositoryMock.Setup(x => x.GetMaterialHistoryByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.MaterialHistory());

        _materialRepositoryMock.Setup(x => x.IsMaterialExist(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        // Act
        Func<Task> act = async () => await _handler.Handle(new UpdateMaterialHistoryCommand(request), default);

        // Assert
        await act.Should().ThrowAsync<MyValidationException>();
    }


}
