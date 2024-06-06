using Application.Abstractions.Data;
using Application.UserCases.Commands.Products.CreateProduct;
using Contract.Services.Product.CreateProduct;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Products.Command;

public class CreateProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductUnitRepository> _productUnitRepositoryMock;
    private readonly Mock<IProductImageRepository> _productImageRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<CreateProductRequest> _validator;
    private readonly List<ProductUnitRequest> _productUnitRequests = new List<ProductUnitRequest>()
    {
        new ProductUnitRequest(Guid.NewGuid(), 4),
        new ProductUnitRequest(Guid.NewGuid(), 4),
    };
    private readonly List<ImageRequest> _imageRequests = new List<ImageRequest>()
    {
        new ImageRequest("Image", true, true)
    };
    public CreateProductCommandHandlerTest()
    {
        _productRepositoryMock = new();
        _productUnitRepositoryMock = new();
        _productImageRepositoryMock = new();
        _unitOfWorkMock = new();
        _validator = new CreateProductValidator(_productRepositoryMock.Object);
    }

    [Theory]
    [InlineData("CD123", 444, "Size1", "Description1", true, "Name1")]
    [InlineData("CD124", 555, "Size2", "Description2", false, "Name2")]
    public async Task Handler_Should_Return_SuccessResult(string code, decimal price, string size,
            string description, bool isGroup, string name)
    {
        var createProductRequest = new CreateProductRequest(code, price, size, description,
            isGroup, name, isGroup ? _productUnitRequests : null, _imageRequests);
        var createProductCommand = new CreateProductCommand(createProductRequest, "createdBy");
        var createProductCommandHandler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _productUnitRepositoryMock.Object,
            _productImageRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator);

        _productRepositoryMock.Setup(p => p.IsHaveGroupInSubProductIds(It.IsAny<List<Guid>>())).ReturnsAsync(false);
        _productRepositoryMock.Setup(p => p.IsProductCodeExist(It.IsAny<string>())).ReturnsAsync(false);

        var result = await createProductCommandHandler.Handle(createProductCommand, default);

        Assert.True(result.isSuccess);
        _productRepositoryMock.Verify(p => p.Add(It.IsAny<Product>()), Times.Once);
        _productUnitRepositoryMock.Verify(p => p.AddRange(It.IsAny<List<ProductUnit>>()), isGroup ? Times.Once : Times.Never);
        _productImageRepositoryMock.Verify(p => p.AddRange(It.IsAny<List<ProductImage>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Failure_When_CodeExists()
    {
        var createProductRequest = new CreateProductRequest("Code", 444, "Size", "Description",
            true, "Name", _productUnitRequests, _imageRequests);
        var createProductCommand = new CreateProductCommand(createProductRequest, "createdBy");
        var createProductCommandHandler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _productUnitRepositoryMock.Object,
            _productImageRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator);

        _productRepositoryMock.Setup(p => p.IsProductCodeExist(It.IsAny<string>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<MyValidationException>(() => createProductCommandHandler.Handle(createProductCommand, default));
    }

    [Theory]
    // Empty Code
    [InlineData("", 444, "Size", "Description", true, "Name")]
    // Invalid Code format
    [InlineData("1C123", 444, "Size", "Description", true, "Name")]
    [InlineData("CC12A", 444, "Size", "Description", true, "Name")]
    [InlineData("CC12345A", 444, "Size", "Description", true, "Name")]
    // Price <= 0
    [InlineData("CC123", 0, "Size", "Description", true, "Name")]
    [InlineData("CC123", -100, "Size", "Description", true, "Name")]
    // Empty Size
    [InlineData("CC123", 444, "", "Description", true, "Name")]
    // Empty Description
    [InlineData("CC123", 444, "Size", "", true, "Name")]
    // Multiple Main Images
    [InlineData("CC123", 444, "Size", "Description", true, "Name", true)]
    // No Main Image
    [InlineData("CC123", 444, "Size", "Description", true, "Name", false, false)]
    public async Task Handler_Should_Return_Failure_When_ValidationFails(string code, decimal price, string size,
            string description, bool isGroup, string name, bool multipleMainImages = false, bool oneMainImage = true)
    {
        List<ImageRequest>? imageRequests = null;
        if (multipleMainImages)
        {
            imageRequests = new List<ImageRequest>()
                {
                    new ImageRequest("Image", true, false),
                    new ImageRequest("Image2", true, false)
                };
        }
        else if (oneMainImage)
        {
            imageRequests = _imageRequests;
        }
        else
        {
            imageRequests = new List<ImageRequest>()
                {
                    new ImageRequest("Image", false, false)
                };
        }

        var createProductRequest = new CreateProductRequest(code, price, size, description,
            isGroup, name, isGroup ? _productUnitRequests : null, imageRequests);
        var createProductCommand = new CreateProductCommand(createProductRequest, "createdBy");
        var createProductCommandHandler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _productUnitRepositoryMock.Object,
            _productImageRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator);

        await Assert.ThrowsAsync<MyValidationException>(() => createProductCommandHandler.Handle(createProductCommand, default));
    }
}
