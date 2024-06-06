using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.UpdateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class UpdateProductTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _productRepository;

    public UpdateProductTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public async Task Update_ProductExists_ShouldUpdateProduct()
    {
        // Arrange
        var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description",
                false, "Name", null, null);
        var product = Product.Create(createProductRequest, "001201011091");
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var updateProductRequest = new UpdateProductRequest(product.Id, "UpdatedCode",
            999, "UpdatedSize", "UpdatedDescription", false, "UpdatedName", true, null, null);
        product.Update(updateProductRequest, "001201012123");

        _productRepository.Update(product);
        await _context.SaveChangesAsync();

        var updatedProduct = await _context.Products.FindAsync(product.Id);

        // Assert
        Assert.NotNull(updatedProduct);
        Assert.Equal("UpdatedCode", updatedProduct.Code);
        Assert.Equal("UpdatedSize", updatedProduct.Size);
        Assert.Equal("UpdatedDescription", updatedProduct.Description);
        Assert.Equal("UpdatedName", updatedProduct.Name);
    }

    [Fact]
    public async Task Update_ProductDoesNotExist_ShouldThrow()
    {
        // Arrange
        var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description",
                false, "Name", null, null);
        var product = Product.Create(createProductRequest, "001201011091");

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () =>
        {
            _productRepository.Update(product);
            await _context.SaveChangesAsync();
        });
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}