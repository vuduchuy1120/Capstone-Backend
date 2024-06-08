using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class GetProductByIdWithoutImagesTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;
    public GetProductByIdWithoutImagesTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productRepository = new ProductRepository(_context);

        var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description", "Name", null);
        var product = Product.Create(createProductRequest, "001201011091");

        _productRepository.Add(product);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProductByIdWithoutImages_ReturnsProduct_WhenProductExists()
    {
        // Arrange
        var productId = _context.Products.First().Id;
        var repository = new ProductRepository(_context);

        // Act
        var result = await repository.GetProductByIdWithoutImages(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Name", result.Name);
    }

    [Fact]
    public async Task GetProductByIdWithoutImages_ReturnsNull_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();
        var repository = new ProductRepository(_context);

        // Act
        var result = await repository.GetProductByIdWithoutImages(nonExistentProductId);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
