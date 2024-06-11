using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.GetProducts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class SearchProductAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;
    public SearchProductAsyncTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productRepository = new ProductRepository(_context);

        var products = new List<Product>
        {
            Product.Create(
                new CreateProductRequest(
                    Code: "P001",
                    Price: 50.00m,
                    Size: "M",
                    Description: "First product",
                    Name: "Product 1",
                    ImageRequests: null // Assuming no images for simplicity
                ),
                createdBy: "User1"
            ),
            Product.Create(
                new CreateProductRequest(
                    Code: "P002",
                    Price: 100.00m,
                    Size: "L",
                    Description: "Second product",
                    Name: "Product 2",
                    ImageRequests: null
                ),
                createdBy: "User2"
            ),
            Product.Create(
                new CreateProductRequest(
                    Code: "P003",
                    Price: 150.00m,
                    Size: "S",
                    Description: "Third product",
                    Name: "Product 3",
                    ImageRequests: null
                ),
                createdBy: "User3"
            )
        };

        // Manually set CreatedDate to preserve the order for testing purposes
        products[0].CreatedDate = DateTime.UtcNow;
        products[1].CreatedDate = DateTime.UtcNow.AddDays(-1);
        products[2].CreatedDate = DateTime.UtcNow.AddDays(-2);

        _context.Products.AddRange(products);
        _context.SaveChanges();

    }
    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SearchProductAsync_ReturnsCorrectResults_WhenFilteredByIsInProcessing()
    {
        // Arrange
        var query = new GetProductsQuery(null, true, 1, 10);
        var repository = new ProductRepository(_context);

        // Act
        var (products, totalPages) = await repository.SearchProductAsync(query);

        // Assert
        Assert.Equal(3, products.Count);
        Assert.Equal(1, totalPages);
        Assert.All(products, p => Assert.True(p.IsInProcessing));
    }

    [Fact]
    public async Task SearchProductAsync_ReturnsCorrectResults_WhenSearchTermIsProvided()
    {
        // Arrange
        var query = new GetProductsQuery("Product 1", true, 1, 10);
        var repository = new ProductRepository(_context);

        // Act
        var (products, totalPages) = await repository.SearchProductAsync(query);

        // Assert
        Assert.Single(products);
        Assert.Equal(1, totalPages);
        Assert.Contains(products, p => p.Name == "Product 1");
    }

    [Fact]
    public async Task SearchProductAsync_ReturnsCorrectResults_WhenPaginated()
    {
        // Arrange
        var query = new GetProductsQuery(null, true, 1, 1);
        var repository = new ProductRepository(_context);

        // Act
        var (products, totalPages) = await repository.SearchProductAsync(query);

        // Assert
        Assert.Single(products);
        Assert.Equal(3, totalPages);
    }
}
