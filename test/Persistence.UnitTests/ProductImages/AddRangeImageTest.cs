using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductImages;

public class AddRangeImageTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductImageRepository _productImageRepository;

    public AddRangeImageTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productImageRepository = new ProductImageRepository(_context);
    }

    [Fact]
    public async Task AddRange_Success_Should_AddProductImagesToDb()
    {
        var productId = Guid.NewGuid();
        var request1 = new ImageRequest("http://example.com/image1.jpg", true, false);
        var request2 = new ImageRequest("http://example.com/image2.jpg", false, true);
        var productImage1 = ProductImage.Create(productId, request1);
        var productImage2 = ProductImage.Create(productId, request2);
        var productImages = new List<ProductImage> { productImage1, productImage2 };

        _productImageRepository.AddRange(productImages);
        await _context.SaveChangesAsync();

        var images = await _context.ProductImages.ToListAsync();
        Assert.Equal(2, images.Count);
        Assert.Contains(images, img => img.ImageUrl == "http://example.com/image1.jpg" && img.IsBluePrint && !img.IsMainImage);
        Assert.Contains(images, img => img.ImageUrl == "http://example.com/image2.jpg" && !img.IsBluePrint && img.IsMainImage);
    }

    [Fact]
    public async Task AddRange_WithNull_Should_ThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _productImageRepository.AddRange(null);
            await _context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task AddRange_WithNullElement_Should_ThrowArgumentException()
    {
        var productId = Guid.NewGuid();
        var request = new ImageRequest("http://example.com/image.jpg", true, false);
        var productImage = ProductImage.Create(productId, request);
        var productImages = new List<ProductImage> { productImage, null };

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _productImageRepository.AddRange(productImages);
            await _context.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task AddRange_WithDuplicateElements_Should_AddAllToDb()
    {
        var productId = Guid.NewGuid();
        var request = new ImageRequest("http://example.com/image.jpg", true, false);
        var productImage = ProductImage.Create(productId, request);
        var productImage_1 = ProductImage.Create(productId, request);
        var productImages = new List<ProductImage> { productImage, productImage_1 };

        _productImageRepository.AddRange(productImages);
        await _context.SaveChangesAsync();

        var images = await _context.ProductImages.ToListAsync();
        Assert.Equal(2, images.Count);
        Assert.All(images, img => Assert.Equal("http://example.com/image.jpg", img.ImageUrl));
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
