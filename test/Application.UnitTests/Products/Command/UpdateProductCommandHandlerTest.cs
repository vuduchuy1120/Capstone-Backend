using Application.Abstractions.Data;
using Application.UserCases.Commands.Products.UpdateProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.Product.UpdateProduct;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Products;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Products.Command;

public class UpdateProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductImageRepository> _productImageRepositoryMock;
    private readonly Mock<IProductPhaseSalaryRepository> _productPhaseSalaryRepositoryMock;
    private readonly Mock<IPhaseRepository> _phaseRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<UpdateProductCommand> _validator;
    private readonly UpdateProductCommandHandler _updateProductCommandHandler;

    public UpdateProductCommandHandlerTest()
    {
        _productImageRepositoryMock = new();
        _productRepositoryMock = new();
        _unitOfWorkMock = new();
        _productPhaseSalaryRepositoryMock = new();
        _phaseRepositoryMock = new();
        _validator = new UpdateProductValidator(_productImageRepositoryMock.Object);
        _updateProductCommandHandler = new UpdateProductCommandHandler(
            _productRepositoryMock.Object,
            _productImageRepositoryMock.Object,
            _phaseRepositoryMock.Object,
            _productPhaseSalaryRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validator);
    }

    [Fact]
    public async Task Handler_Should_ReturnSuccessResult_WhenAllConditionsAreMet()
    {
        var productId = Guid.NewGuid();
        var imagesRequest = new List<ImageRequest> { new ImageRequest("Image1", true, false) };
        var removeImageIds = new List<Guid> { Guid.NewGuid() };
        var updateProductRequest = new UpdateProductRequest(productId, "CD123", 3434, 100, 100, "Size", "Description", "Name", true, imagesRequest, removeImageIds);
        var updateProductCommand = new UpdateProductCommand(updateProductRequest, "UpdatedBy", productId);

        var product = new Product(); // Create a mock product object

        _productRepositoryMock.Setup(repo => repo.GetProductByIdWithoutImages(productId)).ReturnsAsync(product);
        _productRepositoryMock.Setup(repo => repo.IsProductCodeExist(It.IsAny<string>())).ReturnsAsync(false);
        _productImageRepositoryMock.Setup(repo => repo.GetProductImageIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<ProductImage>() { new ProductImage() });
        _productImageRepositoryMock.Setup(repo => repo.IsAllImageIdExist(It.IsAny<List<Guid>>(), It.IsAny<Guid>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _phaseRepositoryMock.Setup(p => p.GetPhases()).ReturnsAsync(new List<Phase>
        {
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_001", "Phase 1")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_002", "Phase 2")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_003", "Phase 3"))

        });
        _productPhaseSalaryRepositoryMock.Setup(repo => repo.GetByProductIdAndPhaseId(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new ProductPhaseSalary());

        var result = await _updateProductCommandHandler.Handle(updateProductCommand, default);

        Assert.True(result.isSuccess);
        _productRepositoryMock.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        _productImageRepositoryMock.Verify(repo => repo.AddRange(It.IsAny<List<ProductImage>>()), Times.Once);
        _productImageRepositoryMock.Verify(repo => repo.DeleteRange(It.IsAny<List<ProductImage>>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_ReturnSuccessResult_WhenRemoveImagesNull()
    {
        var productId = Guid.NewGuid();
        var imagesRequest = new List<ImageRequest> { new ImageRequest("Image1", true, false) };
        var updateProductRequest = new UpdateProductRequest(productId, "CD123", 3434, 100, 100, "Size", "Description", "Name", true, imagesRequest, null);
        var updateProductCommand = new UpdateProductCommand(updateProductRequest, "UpdatedBy", productId);

        var product = new Product(); // Create a mock product object

        _productRepositoryMock.Setup(repo => repo.GetProductByIdWithoutImages(productId)).ReturnsAsync(product);
        _productRepositoryMock.Setup(repo => repo.IsProductCodeExist(It.IsAny<string>())).ReturnsAsync(false);
        _productImageRepositoryMock.Setup(repo => repo.GetProductImageIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<ProductImage>() { new ProductImage() });
        _productImageRepositoryMock.Setup(repo => repo.IsAllImageIdExist(It.IsAny<List<Guid>>(), It.IsAny<Guid>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _phaseRepositoryMock.Setup(p => p.GetPhases()).ReturnsAsync(new List<Phase>
        {
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_001", "Phase 1")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_002", "Phase 2")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_003", "Phase 3"))

        });
        _productPhaseSalaryRepositoryMock.Setup(repo => repo.GetByProductIdAndPhaseId(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new ProductPhaseSalary());
        var result = await _updateProductCommandHandler.Handle(updateProductCommand, default);

        Assert.True(result.isSuccess);
        _productRepositoryMock.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        _productImageRepositoryMock.Verify(repo => repo.AddRange(It.IsAny<List<ProductImage>>()), Times.Once);
        _productImageRepositoryMock.Verify(repo => repo.DeleteRange(It.IsAny<List<ProductImage>>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_ReturnSuccessResult_WhenImageRequestNull()
    {
        var productId = Guid.NewGuid();
        var removeImageIds = new List<Guid> { Guid.NewGuid() };
        var updateProductRequest = new UpdateProductRequest(productId, "CD123", 3434, 100, 100, "Size", "Description", "Name", true, null, removeImageIds);
        var updateProductCommand = new UpdateProductCommand(updateProductRequest, "UpdatedBy", productId);

        var product = new Product(); // Create a mock product object

        _productRepositoryMock.Setup(repo => repo.GetProductByIdWithoutImages(productId)).ReturnsAsync(product);
        _productRepositoryMock.Setup(repo => repo.IsProductCodeExist(It.IsAny<string>())).ReturnsAsync(false);
        _productImageRepositoryMock.Setup(repo => repo.GetProductImageIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<ProductImage>() { new ProductImage() });
        _productImageRepositoryMock.Setup(repo => repo.IsAllImageIdExist(It.IsAny<List<Guid>>(), It.IsAny<Guid>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _phaseRepositoryMock.Setup(p => p.GetPhases()).ReturnsAsync(new List<Phase>
        {
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_001", "Phase 1")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_002", "Phase 2")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_003", "Phase 3"))

        });
        _productPhaseSalaryRepositoryMock.Setup(repo => repo.GetByProductIdAndPhaseId(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new ProductPhaseSalary());

        var result = await _updateProductCommandHandler.Handle(updateProductCommand, default);

        Assert.True(result.isSuccess);
        _productRepositoryMock.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        _productImageRepositoryMock.Verify(repo => repo.AddRange(It.IsAny<List<ProductImage>>()), Times.Never);
        _productImageRepositoryMock.Verify(repo => repo.DeleteRange(It.IsAny<List<ProductImage>>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("", 123, 100, 50, "Size", "Description", "Name")] // Empty code
    [InlineData("CD123", -10, 100, 50, "Size", "Description", "Name")] // Negative price
    [InlineData("123", 123, 100, 50, "Size", "Description", "Name")] // Invalid code format
    [InlineData("CD123", 123, 100, 50, "", "Description", "Name")] // Empty size
    [InlineData("CD123", 123, 100, 50, "Size", "Description", "")] // Empty name
    [InlineData("CD123", 123, 100, 50, "Size", "Description", "Name", new string[] { "Image1", "Image2" }, new bool[] { false, false })] // No main image
    [InlineData("CD123", 123, 100, 50, "Size", "Description", "Name", new string[] { "Image1", "Image2" }, new bool[] { true, true })] // Multiple main images
    public async Task Handler_Should_ThrowValidationException_ForInvalidRequests(
     string code,
     decimal priceFinished,
     decimal pricePhase1,
     decimal pricePhase2,
     string size,
     string description,
     string name,
     string[] imageUrls = null,
     bool[] isMainImages = null)
    {
        var productId = Guid.NewGuid();
        List<ImageRequest> imagesRequest = null;

        if (imageUrls != null && isMainImages != null)
        {
            imagesRequest = new List<ImageRequest>();
            for (int i = 0; i < imageUrls.Length; i++)
            {
                imagesRequest.Add(new ImageRequest(imageUrls[i], isMainImages[i], false));
            }
        }
        _phaseRepositoryMock.Setup(p => p.GetPhases()).ReturnsAsync(new List<Phase>
        {
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_001", "Phase 1")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_002", "Phase 2")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_003", "Phase 3"))

        });
        _productPhaseSalaryRepositoryMock.Setup(repo => repo.GetByProductIdAndPhaseId(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(new ProductPhaseSalary());

        var updateProductRequest = new UpdateProductRequest(productId, code, priceFinished, pricePhase1, pricePhase2, size, description, name, true, imagesRequest, null);
        var updateProductCommand = new UpdateProductCommand(updateProductRequest, "UpdatedBy", productId);

        await Assert.ThrowsAsync<MyValidationException>(() => _updateProductCommandHandler.Handle(updateProductCommand, default));
    }

    [Fact]
    public async Task Handler_Should_ThrowProductNotFoundException_WhenProductDoesNotExist()
    {
        var productId = Guid.NewGuid();
        var updateProductRequest = new UpdateProductRequest(productId, "CD123", 3434, 100, 100, "Size", "Description", "Name", true, null, null);
        var updateProductCommand = new UpdateProductCommand(updateProductRequest, "UpdatedBy", productId);

        _productRepositoryMock.Setup(repo => repo.GetProductByIdWithoutImages(productId)).ReturnsAsync((Product)null);

        var exception = await Assert.ThrowsAsync<ProductNotFoundException>(() => _updateProductCommandHandler.Handle(updateProductCommand, default));

        Assert.IsType<ProductNotFoundException>(exception);
    }

    [Fact]
    public async Task Handler_Should_ThrowProductCodeAlreadyExistException_WhenProductCodeExists()
    {
        var productId = Guid.NewGuid();
        var updateProductRequest = new UpdateProductRequest(productId, "CD123", 3434, 100, 100, "Size", "Description", "Name", true, null, null);
        var updateProductCommand = new UpdateProductCommand(updateProductRequest, "UpdatedBy", productId);

        var existingProduct = new Product(); // Create a mock product object

        _productRepositoryMock.Setup(repo => repo.GetProductByIdWithoutImages(productId)).ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(repo => repo.IsProductCodeExist("CD123")).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<ProductCodeAlreadyExistException>(() => _updateProductCommandHandler.Handle(updateProductCommand, default));

        Assert.IsType<ProductCodeAlreadyExistException>(exception);
    }

    [Fact]
    public async Task Handler_Should_ThrowProductIdConflictException_WhenProductIdMismatch()
    {
        var productId = Guid.NewGuid();
        var updateProductRequest = new UpdateProductRequest(Guid.NewGuid(), "CD123", 3434, 100, 100, "Size", "Description", "Name", true, null, null);
        var updateProductCommand = new UpdateProductCommand(updateProductRequest, "UpdatedBy", productId);

        var exception = await Assert.ThrowsAsync<ProductIdConflictException>(() => _updateProductCommandHandler.Handle(updateProductCommand, default));

        Assert.IsType<ProductIdConflictException>(exception);
    }
}
