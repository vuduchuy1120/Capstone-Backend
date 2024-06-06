using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductImages;

public class DeleteRangeProductImageTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductImageRepository _productImageRepository;

    public DeleteRangeProductImageTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productImageRepository = new ProductImageRepository(_context);
    }
    [Fact]
    public async Task DeleteRange_Should_RemoveNonEmptyList()
    {
        // Arrange
        var productImage1 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));
        var productImage2 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));

        _context.ProductImages.AddRange(productImage1, productImage2);
        await _context.SaveChangesAsync();

        var productImagesToDelete = new List<ProductImage> { productImage1, productImage2 };

        // Act
        _productImageRepository.DeleteRange(productImagesToDelete);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.ProductImages.ToListAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteRange_Should_HandleEmptyList()
    {
        // Arrange
        var initialCount = await _context.ProductImages.CountAsync();

        var productImagesToDelete = new List<ProductImage>();

        // Act
        _productImageRepository.DeleteRange(productImagesToDelete);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.ProductImages.CountAsync();
        Assert.Equal(initialCount, result);
    }

    [Fact]
    public async Task DeleteRange_Should_RemoveOnlySpecifiedImages()
    {
        // Arrange
        var productImage1 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));
        var productImage2 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));
        var productImage3 = ProductImage.Create(Guid.NewGuid(), new ImageRequest("Image", true, false));

        _context.ProductImages.AddRange(productImage1, productImage2, productImage3);
        await _context.SaveChangesAsync();

        var productImagesToDelete = new List<ProductImage> { productImage1, productImage2 };

        // Act
        _productImageRepository.DeleteRange(productImagesToDelete);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.ProductImages.ToListAsync();
        Assert.Single(result);
        Assert.Contains(result, p => p.Id == productImage3.Id);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
