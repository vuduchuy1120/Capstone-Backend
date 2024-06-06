using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductImages;

public class GetProductImageIdsAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductImageRepository _productImageRepository;

    public GetProductImageIdsAsyncTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productImageRepository = new ProductImageRepository(_context);
    }
    [Fact]
    public async Task GetProductImageIdsAsync_Should_ReturnCorrectProductImages()
    {
        // Arrange
        var productImage1 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));
        var productImage2 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));
        var productImage3 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));

        _context.ProductImages.AddRange(productImage1, productImage2, productImage3);
        await _context.SaveChangesAsync();

        var productImageIds = new List<Guid> { productImage1.Id, productImage3.Id };

        // Act
        var result = await _productImageRepository.GetProductImageIdsAsync(productImageIds);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == productImage1.Id);
        Assert.Contains(result, p => p.Id == productImage3.Id);
        Assert.DoesNotContain(result, p => p.Id == productImage2.Id);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
