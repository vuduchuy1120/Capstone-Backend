using Application.Abstractions.Data;
using Application.UserCases.Commands.Products.CreateProduct;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.SharedDto;
using Contract.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Products.Command;

public class CreateProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductImageRepository> _productImageRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<CreateProductRequest> _validator;
    private readonly CreateProductCommandHandler _createProductCommandHandler;
    public CreateProductCommandHandlerTest()
    {
        _productImageRepositoryMock = new();
        _productRepositoryMock = new();
        _unitOfWorkMock = new();
        _validator = new CreateProductValidator(_productRepositoryMock.Object);
        _createProductCommandHandler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _productImageRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator);
    }

    [Fact]
    public async Task Handler_Should_ReturnSuccessResult_WhenProductImageNotNull()
    {
        var imagesRequest = new List<ImageRequest>
        {
            new ImageRequest("Image", true, false),
            new ImageRequest("Image", false, true)
        };
        var createProductRequest = new CreateProductRequest("CD123", 123, "Size", "Description", "Name", imagesRequest);
        var createProductCommand = new CreateProductCommand(createProductRequest, "CreatedBy");

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _createProductCommandHandler.Handle(createProductCommand, default);

        Assert.True(result.isSuccess);

        _productImageRepositoryMock.Verify(pImage => pImage.AddRange(It.IsAny<List<ProductImage>>()), Times.Once);
        _productRepositoryMock.Verify(p => p.Add(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_ReturnSuccessResult_WhenProductImageNull()
    {
        var createProductRequest = new CreateProductRequest("CD123", 123, "Size", "Description", "Name", null);
        var createProductCommand = new CreateProductCommand(createProductRequest, "CreatedBy");

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        var result = await _createProductCommandHandler.Handle(createProductCommand, default);

        Assert.True(result.isSuccess);

        _productImageRepositoryMock.Verify(pImage => pImage.AddRange(It.IsAny<List<ProductImage>>()), Times.Never);
        _productRepositoryMock.Verify(p => p.Add(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Theory]
    [InlineData("", 123, "Size", "Description", "Name")] // Empty code
    [InlineData("CD123", -10, "Size", "Description", "Name")] // Negative price
    [InlineData("DFSDF123", 123, "Size", "Description", "Name")] // Invalid code format
    [InlineData("DSFSDF", 123, "Size", "Description", "Name")] // Invalid code format
    [InlineData("123", 123, "Size", "Description", "Name")] // Invalid code format
    [InlineData("CD123", 123, "", "Description", "Name")] // Empty size
    [InlineData("CD123", 123, "Size", "", "Name")] // Empty description
    [InlineData("CD123", 123, "Size", "Description", "")] // Empty name
    [InlineData("CD123", 123, "Size", "Description", "Name", new string[] { "Image1", "Image2" }, new bool[] { false, false })] // No main image
    [InlineData("CD123", 123, "Size", "Description", "Name", new string[] { "Image1", "Image2" }, new bool[] { true, true })] // Multiple main images
    public async Task Handler_Should_ThrowValidationException_ForInvalidRequests(
        string code,
        decimal price,
        string size,
        string description,
        string name,
        string[] imageUrls = null,
        bool[] isMainImages = null)
    {
        List<ImageRequest> imagesRequest = null;
        if (imageUrls != null && isMainImages != null)
        {
            imagesRequest = new List<ImageRequest>();
            for (int i = 0; i < imageUrls.Length; i++)
            {
                imagesRequest.Add(new ImageRequest(imageUrls[i], isMainImages[i], false));
            }
        }

        var createProductRequest = new CreateProductRequest(code, price, size, description, name, imagesRequest);
        var createProductCommand = new CreateProductCommand(createProductRequest, "CreatedBy");

        await Assert.ThrowsAsync<MyValidationException>(() => _createProductCommandHandler.Handle(createProductCommand, default));
    }
}
