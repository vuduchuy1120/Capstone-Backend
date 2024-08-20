using Application.Abstractions.Data;
using Application.UserCases.Commands.Materials.Update;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Material.Update;
using Domain.Abstractions.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Materials.Command;

public class UpdateMaterialCommandHandlerTests
{
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<UpdateMaterialRequest> _validator;

    private readonly UpdateMaterialCommandHandler _handler;

    public UpdateMaterialCommandHandlerTests()
    {
        _materialRepositoryMock = new Mock<IMaterialRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new UpdateMaterialRequestValidator(_materialRepositoryMock.Object);
        _handler = new UpdateMaterialCommandHandler(
                            _materialRepositoryMock.Object,
                            _unitOfWorkMock.Object,
                            _validator);
    }

    [Fact]
    public async Task Handle_Should_Return_SuccessResult()
    {
        // Arrange
        var request = new UpdateMaterialRequest(
                                  Id: Guid.NewGuid(),
                                  Name: "Material 1",
                                  Description: "Description 1",
                                  Unit: "Unit 1",
                                  QuantityPerUnit: 1,
                                  Image: "No Image",
                                  QuantityInStock: 1);

        _materialRepositoryMock.Setup(x => x.IsMaterialExist(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _materialRepositoryMock.Setup(x => x.GetMaterialByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.Material());
        _materialRepositoryMock.Setup(x=>x.IsMaterialNameExistedAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(new UpdateMaterialCommand(request), CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result.Success>();
    }

    [Theory]
    [InlineData(1, "", "Description 1", "Unit 1", 1, "No Image", 2)]
    [InlineData(1, "Material 1", "Description 1", "", 1, "No Image",2)]
    [InlineData(1, "Material 1", "Description 1", "Unit 1", 0, "No Image",2)]
    [InlineData(1, "Material 1", "Description 1", "Unit 1", -1, "",2)]
    [InlineData(1, "Material 1", "Description 1", "Unit 1", 1, "No Imange", -2)]

    public async Task Handle_Should_Throw_ValidationException(
        int id, 
        string name,
        string description,
        string unit,
        int quantityPerUnit, 
        string image,
        double quantityInStock)
    {
        // Arrange
        var request = new UpdateMaterialRequest(
                                 Id: Guid.NewGuid(),
                                 Name: name,
                                 Description: description,
                                 Unit: unit,
                                 QuantityPerUnit: quantityPerUnit,
                                 Image: image,
                                 QuantityInStock: quantityInStock);

        // Act
        _materialRepositoryMock.Setup(x => x.IsMaterialExist(It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _materialRepositoryMock.Setup(x => x.GetMaterialByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.Material());
        _materialRepositoryMock.Setup(x => x.IsMaterialNameExistedAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        Func<Task> act = async () => await _handler.Handle(new UpdateMaterialCommand(request), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MyValidationException>();
    }
}
