using Application.Abstractions.Data;
using Application.UserCases.Commands.Products.CreateProduct;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.SharedDto;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.UnitTests.Products.Command;

public class CreateProductCommandHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductImageRepository> _productImageRepositoryMock;
    private readonly Mock<IProductPhaseSalaryRepository> _productPhaseSalaryRepository;
    private readonly Mock<IPhaseRepository> _phaseRepository;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IValidator<CreateProductRequest> _validator;
    private readonly CreateProductCommandHandler _createProductCommandHandler;
    public CreateProductCommandHandlerTest()
    {
        _productImageRepositoryMock = new();
        _productRepositoryMock = new();
        _productPhaseSalaryRepository = new();
        _phaseRepository = new();
        _unitOfWorkMock = new();
        _validator = new CreateProductValidator(_productRepositoryMock.Object);
        _createProductCommandHandler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _productImageRepositoryMock.Object,
            _productPhaseSalaryRepository.Object,
            _phaseRepository.Object,
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
        var createProductRequest = new CreateProductRequest("CD123", 123, 1, 2, "Size", "Description", "Name", imagesRequest);
        var createProductCommand = new CreateProductCommand(createProductRequest, "CreatedBy");

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);
        _phaseRepository.Setup(p => p.GetPhases()).ReturnsAsync(new List<Phase>
        {
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_001", "Phase 1")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_002", "Phase 2")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_003", "Phase 3"))

        });

        var result = await _createProductCommandHandler.Handle(createProductCommand, default);

        Assert.True(result.isSuccess);

        _productImageRepositoryMock.Verify(pImage => pImage.AddRange(It.IsAny<List<ProductImage>>()), Times.Once);
        _productRepositoryMock.Verify(p => p.Add(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_ThrowMyValidationException_WhenProductImageNull()
    {
        var createProductRequest = new CreateProductRequest("CD123", 100, 10, 10, "Size", "Description", "Name", null);
        var createProductCommand = new CreateProductCommand(createProductRequest, "CreatedBy");

        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(default)).ReturnsAsync(1);

        await Assert.ThrowsAsync<MyValidationException>(() =>
        _createProductCommandHandler.Handle(createProductCommand, default));
    }

    [Theory]
    [InlineData("", 123, 100, 150, "Size", "Description", "Name")] // Empty code
    [InlineData("CD123", -10, 100, 150, "Size", "Description", "Name")] // Negative price
    [InlineData("CD123", 100, -10, 150, "Size", "Description", "Name")] // Negative price phase1
    [InlineData("CD123", 100, 150, -10, "Size", "Description", "Name")] // Negative price phase2
    [InlineData("CD123", 100, 50, 150, "Size", "Description", "Name")] // Price phase2 greater than price phase1
    [InlineData("CD123", 123, 100, 150, "", "Description", "Name")] // Empty size
    [InlineData("CD123", 123, 100, 150, "Size", "", "Name")] // Empty description
    [InlineData("CD123", 123, 100, 150, "Size", "Description", "")] // Empty name
    [InlineData("CD123", 123, 100, 150, "Size", "Description", "Name", new string[] { "Image1", "Image2" }, new bool[] { false, false })] // No main image
    [InlineData("CD123", 123, 100, 150, "Size", "Description", "Name", new string[] { "Image1", "Image2" }, new bool[] { true, true })] // Multiple main images
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
        List<ImageRequest> imagesRequest = null;
        if (imageUrls != null && isMainImages != null)
        {
            imagesRequest = new List<ImageRequest>();
            for (int i = 0; i < imageUrls.Length; i++)
            {
                imagesRequest.Add(new ImageRequest(imageUrls[i], isMainImages[i], false));
            }
        }
        _phaseRepository.Setup(p => p.GetPhases()).ReturnsAsync(new List<Phase>
        {
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_001", "Phase 1")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_002", "Phase 2")),
            Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_003", "Phase 3"))

        });
        var createProductRequest = new CreateProductRequest(code, priceFinished, pricePhase1, pricePhase2, size, description, name, imagesRequest);
        var createProductCommand = new CreateProductCommand(createProductRequest, "CreatedBy");

        await Assert.ThrowsAsync<MyValidationException>(() => _createProductCommandHandler.Handle(createProductCommand, default));
    }

}
