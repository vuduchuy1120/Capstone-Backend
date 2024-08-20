using Application.Abstractions.Data;
using Application.UserCases.Queries.Products.GetProducts;
using AutoMapper;
using Contract.Services.Product.GetProducts;
using Domain.Entities;
using Domain.Exceptions.Products;
using Moq;

namespace Application.UnitTests.Products.Query;

public class GetProductsQueryHandlerTest
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProductsQueryHandler _getProductsQueryHandler;

    public GetProductsQueryHandlerTest()
    {
        _productRepositoryMock = new();
        _mapperMock = new();
        _getProductsQueryHandler = new GetProductsQueryHandler(_productRepositoryMock.Object);
    }

    //[Fact]
    //public async Task Handle_Should_ReturnSuccessResult_WithSearchResponse_WhenProductsExist()
    //{
    //    // Arrange
    //    var query = new GetProductsQuery("Test", true, 1, 10);

    //    var products = new List<Product>
    //        {
    //            Product.Create(new CreateProductRequest("Code", 123,10,10, "Size", "Description", "Name", null), "CreatedBy"),
    //            Product.Create(new CreateProductRequest("Code", 123,10,10, "Size", "Description", "Name", null), "CreatedBy"),
    //        };

    //    var totalPage = 1;

    //    var expectedProductResponses = new List<ProductResponse>
    //        {
    //            new ProductResponse(products[0].Id, products[0].Name, products[0].Code, products[0].Price,
    //            products[0].ProductPhaseSalaries.Select(salary => new ProductPhaseSalaryResponse(
    //            salary.PhaseId,
    //            salary.Phase.Name,
    //            salary.SalaryPerProduct)).ToList(),
    //                products[0].Size, products[0].Description,products[0].IsInProcessing, null),
    //            new ProductResponse(products[1].Id, products[1].Name, products[1].Code, products[1].Price,
    //            products[1].ProductPhaseSalaries.Select(salary => new ProductPhaseSalaryResponse(
    //            salary.PhaseId,
    //            salary.Phase.Name,
    //            salary.SalaryPerProduct)).ToList(),
    //                products[1].Size, products[1].Description,products[1].IsInProcessing, null),
    //        };

    //    _productRepositoryMock.Setup(repo => repo.SearchProductAsync(query)).ReturnsAsync((products, totalPage));
    //    _mapperMock.Setup(mapper => mapper.Map<List<ProductResponse>>(products)).Returns(expectedProductResponses);

    //    // Act
    //    var result = await _getProductsQueryHandler.Handle(query, CancellationToken.None);

    //    // Assert
    //    Assert.True(result.isSuccess);
    //    Assert.Equal(totalPage, result.data.TotalPages);
    //    Assert.Equal(query.PageIndex, result.data.CurrentPage);
    //}

    [Fact]
    public async Task Handle_Should_ThrowProductNotFoundException_WhenNoProductsExist()
    {
        // Arrange
        var query = new GetProductsQuery("Test", true, 1, 10);

        _productRepositoryMock.Setup(repo => repo.SearchProductAsync(query)).ReturnsAsync((new List<Product>(), 0));

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _getProductsQueryHandler.Handle(query, CancellationToken.None));
    }
}
