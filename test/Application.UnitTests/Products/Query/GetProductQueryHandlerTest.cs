using Application.Abstractions.Data;
using Application.UserCases.Queries.Products.GetProduct;
using AutoMapper;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.GetProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.ProductPhaseSalary.ShareDtos;
using Domain.Entities;
using Domain.Exceptions.Products;
using Moq;

namespace Application.UnitTests.Products.Query;

public class GetProductQueryHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProductQueryHandler _getProductQueryHandler;

    public GetProductQueryHandlerTest()
    {
        _productRepositoryMock = new();
        _mapperMock = new();
        _getProductQueryHandler = new GetProductQueryHandler(_productRepositoryMock.Object);
    }

    //[Fact]
    //public async Task Handle_Should_ReturnSuccessResult_WithProductResponse_WhenProductExists()
    //{
    //    // Arrange

    //    // create phases
    //    var phase1 = Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_001", "Phase 1"));
    //    var phase2 = Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_002", "Phase 2"));
    //    var phase3 = Phase.Create(new Contract.Services.Phase.Creates.CreatePhaseRequest("PH_003", "Phase 3"));
    //    var product = Product.Create(new CreateProductRequest("P001", 100, 10, 10, "M", "Test Description",
    //        "Test Product", null), "TestUser");

    //    var expectedProductResponse = new ProductWithTotalQuantityResponse(
    //        product.Id,
    //        product.Name, 
    //        product.Code, 
    //        product.Price,
    //        null,
    //        null,
    //         product.Size, product.Description, product.IsInProcessing, null);

    //    _productRepositoryMock.Setup(repo => repo.GetProductByIdWithProductPhase(product.Id)).ReturnsAsync(product);

    //    // Act
    //    var result = await _getProductQueryHandler.Handle(new GetProductQuery(product.Id), CancellationToken.None);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.True(result.isSuccess);
    //}

    [Fact]
    public async Task Handle_Should_ThrowProductNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _productRepositoryMock.Setup(repo => repo.GetProductById(productId)).ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _getProductQueryHandler.Handle(new GetProductQuery(productId), CancellationToken.None));
    }
}
