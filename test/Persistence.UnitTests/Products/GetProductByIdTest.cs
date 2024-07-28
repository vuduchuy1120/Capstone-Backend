using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class GetProductByIdTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _productRepository;

    public GetProductByIdTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public async Task GetProductById_ProductExists_ShouldReturnProduct()
    {
        // Arrange
        var createProductRequest = new CreateProductRequest("Code", 123, 10, 10, "Size", "Description",
            "Name", null);
        var product = Product.Create(createProductRequest, "001201011091");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.GetProductById(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Code", result.Code);
        Assert.Equal("Size", result.Size);
        Assert.Equal("Description", result.Description);
        Assert.Equal("Name", result.Name);
    }

    [Fact]
    public async Task GetProductById_ProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var result = await _productRepository.GetProductById(productId);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
