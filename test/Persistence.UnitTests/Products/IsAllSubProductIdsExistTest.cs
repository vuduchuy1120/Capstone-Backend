using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class IsAllSubProductIdsExistTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _productRepository;

    public IsAllSubProductIdsExistTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public async Task IsAllSubProductIdsExist_AllIdsExist_ShouldReturnTrue()
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
                "Name", null);
            var product = Product.Create(createProductRequest, "001201011091");
            product.Id = id;
            return product;
        }).ToList();

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.IsAllSubProductIdsExist(subProductIds);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAllSubProductIdsExist_SomeIdsDoNotExist_ShouldReturnFalse()
    {
        // Arrange
        var existingSubProductIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

        var nonExistingSubProductIds = new List<Guid>
            {
                Guid.NewGuid()
            };

        var subProductIds = existingSubProductIds.Concat(nonExistingSubProductIds).ToList();

        var products = existingSubProductIds.Select(id =>
        {
            var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description",
                "Name", null);
            var product = Product.Create(createProductRequest, "001201011091");
            product.Id = id;
            return product;
        }).ToList();

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productRepository.IsAllSubProductIdsExist(subProductIds);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}