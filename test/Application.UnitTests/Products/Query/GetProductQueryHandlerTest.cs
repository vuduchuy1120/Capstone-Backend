using Application.Abstractions.Data;
using Application.UserCases.Queries.Products.GetProduct;
using AutoMapper;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.GetProduct;
using Contract.Services.Product.SharedDto;
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
        _getProductQueryHandler = new GetProductQueryHandler(_productRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WithProductResponse_WhenProductExists()
    {
        // Arrange
        var product = Product.Create(new CreateProductRequest("P001", 100, "M", "Test Description",
            "Test Product", null), "TestUser");

        var expectedProductResponse = new ProductResponse( product.Id, product.Name,product.Code,product.Price,
             product.Size,product.Description,product.IsInProcessing, null);

        _productRepositoryMock.Setup(repo => repo.GetProductById(product.Id)).ReturnsAsync(product);
        _mapperMock.Setup(mapper => mapper.Map<ProductResponse>(product)).Returns(expectedProductResponse);

        // Act
        var result = await _getProductQueryHandler.Handle(new GetProductQuery(product.Id), CancellationToken.None);

        // Assert
        Assert.True(result.isSuccess);
        Assert.Equal(expectedProductResponse, result.data);
    }

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
