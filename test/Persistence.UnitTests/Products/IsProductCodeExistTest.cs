using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class IsProductCodeExistTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _productRepository;

    public IsProductCodeExistTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public async Task IsProductCodeExist_CodeExists_ShouldReturnTrue()
    {
        // Arrange
        var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description",
                "Name", null);
        var product = Product.Create(createProductRequest, "001201011091");

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.IsProductCodeExist(product.Code);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsProductCodeExist_CodeDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _productRepository.IsProductCodeExist("NonExistingCode");

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}