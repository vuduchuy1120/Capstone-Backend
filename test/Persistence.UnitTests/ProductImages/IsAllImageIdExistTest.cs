using Application.Abstractions.Data;
using Contract.Services.Product.SharedDto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductImages;

public class IsAllImageIdExistTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductImageRepository _productImageRepository;

    public IsAllImageIdExistTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productImageRepository = new ProductImageRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task IsAllImageIdExist_ReturnsTrue_WhenAllImageIdsExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var imageIds = new List<Guid>();

        for(int i = 0; i < 5; i++)
        {
            var imageRequest = new ImageRequest("http://example.com/image.jpg", false, false);
            var productImage = ProductImage.Create(productId, imageRequest);
            _context.ProductImages.Add(productImage);
            imageIds.Add(productImage.Id);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _productImageRepository.IsAllImageIdExist(imageIds, productId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAllImageIdExist_ReturnsFalse_WhenSomeImageIdsDoNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingImageId = Guid.NewGuid();
        var nonExistingImageId = Guid.NewGuid();
        var imageIds = new List<Guid> { existingImageId, nonExistingImageId };

        var imageRequest = new ImageRequest("http://example.com/image.jpg", false, false);
        var productImage = ProductImage.Create(productId, imageRequest);
        _context.ProductImages.Add(productImage);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productImageRepository.IsAllImageIdExist(imageIds, productId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsAllImageIdExist_ReturnsFalse_WhenNoImageIdsExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var imageIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await _productImageRepository.IsAllImageIdExist(imageIds, productId);

        // Assert
        Assert.False(result);
    }
}
