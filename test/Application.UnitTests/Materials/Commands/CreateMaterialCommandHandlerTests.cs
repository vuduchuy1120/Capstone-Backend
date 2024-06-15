using Application.Abstractions.Data;
using Application.UserCases.Commands.Materials.Create;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Material.Create;
using Contract.Abstractions.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Materials.Command;

public class CreateMaterialCommandHandlerTests
{
    private readonly Mock<IMaterialRepository> _materialRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<CreateMaterialRequest> _validator;
    private readonly CreateMaterialCommandHandler _handler;

    public CreateMaterialCommandHandlerTests()
    {
        _materialRepositoryMock = new Mock<IMaterialRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new CreateMaterialRequestValidator(_materialRepositoryMock.Object);
        _handler = new CreateMaterialCommandHandler(
                       _materialRepositoryMock.Object,
                       _unitOfWorkMock.Object,
                       _validator);
    }

    [Fact]
    public async Task Handle_Should_Return_SuccessResult()
    {
        // Arrange
        var request = new CreateMaterialRequest(
                       Name: "Material 1",
                       Description: "Description 1",
                       Unit: "Unit 1",
                       QuantityPerUnit: 1,
                       Image: "No Image");


        // Act
        var result = await _handler.Handle(new CreateMaterialCommand(request), CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result.Success>();
    }
    [Theory]
    [InlineData("", "Description 1", "Unit 1", 1, "No Image")]
    [InlineData("Material 1", "", "", 1, "No Image")]
    [InlineData("Material 1", "Description 1", "", 1, "No Image")]
    [InlineData("Material 1", "Description 1", "Unit 1", 0, "No Image")]
    [InlineData("Material 1", "Description 1", "Unit 1", -1, "")]
    public async Task Handle_Should_Throw_ValidationException(string name, string description, string unit, int quantityPerUnit, string image)
    {
        // Arrange
        var request = new CreateMaterialRequest(
                      Name: name,
                      Description: description,
                      Unit: unit,
                      QuantityPerUnit: quantityPerUnit,
                      Image: image);

        // Act
        Func<Task> act = async () => await _handler.Handle(new CreateMaterialCommand(request), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<MyValidationException>();
    }

}
