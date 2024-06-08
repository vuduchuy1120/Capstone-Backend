using Application.Abstractions.Data;
using Contract.Services.Product.SharedDto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductImages;

public class DeleteImageTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductImageRepository _productImageRepository;

    public DeleteImageTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productImageRepository = new ProductImageRepository(_context);
    }

    [Fact]
    public async Task DeleteImage_Success_Should_RemoveProductImageFromDb()
    {
        var productId = Guid.NewGuid();
        var request = new ImageRequest("http://example.com/image.jpg", true, false);
        var productImage = ProductImage.Create(productId, request);
        _productImageRepository.Add(productImage);
        await _context.SaveChangesAsync();

        _productImageRepository.Delete(productImage);
        await _context.SaveChangesAsync();

        var images = _context.ProductImages.ToList();
        Assert.Empty(images);
    }

    [Fact]
    public async Task DeleteImage_NotExisting_Should_Throw_DbUpdateConcurrencyException()
    {
        var productId = Guid.NewGuid();
        var request = new ImageRequest("http://example.com/image.jpg", true, false);
        var productImage = ProductImage.Create(productId, request);
        _productImageRepository.Add(productImage);
        await _context.SaveChangesAsync();

        var productImageNotInDb = ProductImage.Create(productId, request);

        await Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException>(async () =>
        {
            _productImageRepository.Delete(productImageNotInDb);
            await _context.SaveChangesAsync();
        });

        var count = await _context.ProductImages.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task DeleteImage_Null_Should_ThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _productImageRepository.Delete(null);
            await _context.SaveChangesAsync();
        });
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
