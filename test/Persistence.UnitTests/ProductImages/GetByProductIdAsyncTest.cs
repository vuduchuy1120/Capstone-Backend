using Application.Abstractions.Data;
using Contract.Services.Product.SharedDto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductImages;

public class GetByProductIdAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductImageRepository _productImageRepository;

    public GetByProductIdAsyncTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productImageRepository = new ProductImageRepository(_context);
    }
    [Fact]
    public async Task GetByProductIdAsync_Success_Should_ReturnProductImages()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request1 = new ImageRequest("http://example.com/image1.jpg", true, false);
        var request2 = new ImageRequest("http://example.com/image2.jpg", false, true);
        var productImage1 = ProductImage.Create(productId, request1);
        var productImage2 = ProductImage.Create(productId, request2);
        _productImageRepository.Add(productImage1);
        _productImageRepository.Add(productImage2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productImageRepository.GetByProductIdAsync(productId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, img => img.ImageUrl == "http://example.com/image1.jpg" && img.IsBluePrint && !img.IsMainImage);
        Assert.Contains(result, img => img.ImageUrl == "http://example.com/image2.jpg" && !img.IsBluePrint && img.IsMainImage);
    }

    [Fact]
    public async Task GetByProductIdAsync_NoImages_Should_ReturnEmptyList()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var result = await _productImageRepository.GetByProductIdAsync(productId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByProductIdAsync_InvalidProductId_Should_ReturnEmptyList()
    {
        // Arrange
        var invalidProductId = Guid.Empty;

        // Act
        var result = await _productImageRepository.GetByProductIdAsync(invalidProductId);

        // Assert
        Assert.Empty(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
