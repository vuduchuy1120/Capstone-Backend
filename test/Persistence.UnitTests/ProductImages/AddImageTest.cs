using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Persistence.Repositories;
using Application.Abstractions.Data;
using Contract.Services.Product.SharedDto;

namespace Persistence.UnitTests.ProductImages
{
    public class AddImageTest : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IProductImageRepository _productImageRepository;

        public AddImageTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _productImageRepository = new ProductImageRepository(_context);
        }

        [Fact]
        public void AddImage_Success_Should_AddNewProductImageToDb()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new ImageRequest("http://example.com/image.jpg", true, false);
            var productImage = ProductImage.Create(productId, request);

            // Act
            _productImageRepository.Add(productImage);
            _context.SaveChanges();

            // Assert
            var images = _context.ProductImages.ToList();
            Assert.Single(images);
            Assert.Equal(productId, images[0].ProductId);
            Assert.Equal("http://example.com/image.jpg", images[0].ImageUrl);
            Assert.True(images[0].IsBluePrint);
            Assert.False(images[0].IsMainImage);
        }

        [Fact]
        public async Task AddImage_EmptyUrl_Should_ThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new ImageRequest(null, true, false);
            var productImage = ProductImage.Create(productId, request);

            // Act & Assert
            await Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(async () =>
            {
                _productImageRepository.Add(productImage);
                await _context.SaveChangesAsync();
            });
        }

        [Fact]
        public void AddImage_WithVariousBooleanProperties_Should_AddNewProductImageToDb()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request1 = new ImageRequest("http://example.com/image1.jpg", true, false);
            var request2 = new ImageRequest("http://example.com/image2.jpg", false, true);
            var productImage1 = ProductImage.Create(productId, request1);
            var productImage2 = ProductImage.Create(productId, request2);

            // Act
            _productImageRepository.Add(productImage1);
            _productImageRepository.Add(productImage2);
            _context.SaveChanges();

            // Assert
            var images = _context.ProductImages.ToList();
            Assert.Equal(2, images.Count);
            Assert.Contains(images, img => img.ImageUrl == "http://example.com/image1.jpg" && img.IsBluePrint && !img.IsMainImage);
            Assert.Contains(images, img => img.ImageUrl == "http://example.com/image2.jpg" && !img.IsBluePrint && img.IsMainImage);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
