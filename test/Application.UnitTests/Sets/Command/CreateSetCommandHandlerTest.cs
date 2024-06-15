using Application.Abstractions.Data;
using Application.UserCases.Commands.Sets.CreateSet;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.SharedDto;
using Contract.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;
using System.Collections.Specialized;

namespace Application.UnitTests.Sets.Command;

public class CreateSetCommandHandlerTest
{
    private readonly Mock<ISetRepository> _setRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly IValidator<CreateSetRequest> _validator;
    private readonly CreateSetCommandHandler createSetCommandHandler;
    public CreateSetCommandHandlerTest()
    {
        _setRepositoryMock = new();
        _unitOfWorkMock = new();
        _productRepositoryMock = new();
        _validator = new CreateSetValidator(_setRepositoryMock.Object, _productRepositoryMock.Object);
        createSetCommandHandler = new(
            _setRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator);
    }

    [Fact]
    public async Task Handler_Success_ShouldAddNewSet_WhenSetProductRequestIsNull()
    {
        // Arrange
        var createSetRequest = new CreateSetRequest("CD123", "Name", "Description", "Image", null);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");

        _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await createSetCommandHandler.Handle(createSetCommand, default);

        // Assert
        Assert.True(result.isSuccess);
        _setRepositoryMock.Verify(p => p.Add(It.IsAny<Set>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handler_Success_ShouldAddNewSet_WhenSetProductRequestIsNotNull()
    {
        // Arrange
        var setProductsRequest = new List<SetProductRequest>()
        {
            new SetProductRequest(Guid.NewGuid(), 5),
            new SetProductRequest(Guid.NewGuid(), 5)
        };

        var createSetRequest = new CreateSetRequest("CD123", "Name", "Description", "Image", setProductsRequest);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");

        _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>())).ReturnsAsync(true);

        // Act
        var result = await createSetCommandHandler.Handle(createSetCommand, default);

        // Assert
        Assert.True(result.isSuccess);
        _setRepositoryMock.Verify(p => p.Add(It.IsAny<Set>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handler_Fail_ShouldThrowMyValidationException_WhenCodeIsExist()
    {
        // Arrange
        var createSetRequest = new CreateSetRequest("CD123", "Name", "Description", "Image", null);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");

        _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>())).ReturnsAsync(true);


        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            var result = await createSetCommandHandler.Handle(createSetCommand, default);
        });
    }

    [Fact]
    public async Task Handler_Fail_ShouldThrowMyValidationException_WhenSomeProductIdNotExist()
    {
        // Arrange
        var setProductsRequest = new List<SetProductRequest>()
        {
            new SetProductRequest(Guid.NewGuid(), 5),
            new SetProductRequest(Guid.NewGuid(), 5)
        };

        var createSetRequest = new CreateSetRequest("CD123", "Name", "Description", "Image", setProductsRequest);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");

        _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>())).ReturnsAsync(false);
        _productRepositoryMock.Setup(repo => repo.IsAllSubProductIdsExist(It.IsAny<List<Guid>>())).ReturnsAsync(false) ;

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            var result = await createSetCommandHandler.Handle(createSetCommand, default);
        });
    }

    [Theory]
    [InlineData("", "Name", "Description", "Image")] 
    [InlineData("DFFD", "Name", "Description", "Image")]
    [InlineData("2131", "Name", "Description", "Image")]
    [InlineData("223FD", "Name", "Description", "Image")]
    [InlineData("D34", "Name", "Description", "Image")]
    [InlineData(null, "Name", "Description", "Image")]
    [InlineData("CD123", "", "Description", "Image")]
    [InlineData("CD123", null, "Description", "Image")]
    [InlineData("CD123", "Name", "", "Image")]
    [InlineData("CD123", "Name", null, "Image")]
    [InlineData("CD123", "Name", "Description", "")]
    [InlineData("CD123", "Name", "Description", null)]
    [InlineData("", "", "", "")]
    public async Task Handler_Fail_ShouldThrowMyValidationException_WhenRequestInvalid(
        string code, string name, string description, string image)
    {
        // Arrange
        var createSetRequest = new CreateSetRequest(code, name, description, image, null);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");

        _setRepositoryMock.Setup(repo => repo.IsCodeExistAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<MyValidationException>(async () =>
        {
            var result = await createSetCommandHandler.Handle(createSetCommand, default);
        });
    }
}
