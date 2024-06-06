using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class IsHaveGroupInSubProductIdsTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _productRepository;

    public IsHaveGroupInSubProductIdsTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public async Task IsHaveGroupInSubProductIds_HaveGroup_ShouldReturnTrue()
    {
        // Arrange
        var subProductIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

        var products = subProductIds.Select((id, index) =>
        {
            var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description",
                index == 1, "Name", null, null);
            var product = Product.Create(createProductRequest, "001201011091");
            product.Id = id;
            return product;
        }).ToList();

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.IsHaveGroupInSubProductIds(subProductIds);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHaveGroupInSubProductIds_NoGroup_ShouldReturnFalse()
    {
        // Arrange
        var subProductIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

        var products = subProductIds.Select(id =>
        {
            var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description",
                false, "Name", null, null);
            var product = Product.Create(createProductRequest, "001201011091");
            product.Id = id;
            return product;
        }).ToList();

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.IsHaveGroupInSubProductIds(subProductIds);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}