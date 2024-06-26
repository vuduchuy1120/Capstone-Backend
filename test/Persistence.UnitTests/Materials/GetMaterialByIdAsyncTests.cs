using Contract.Services.Material.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.Materials
{
    public class GetMaterialByIdAsyncTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialRepository _materialRepository;

        public GetMaterialByIdAsyncTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _materialRepository = new MaterialRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetMaterialByIdAsync_WithExistingMaterialId_ShouldReturnMaterial()
        {
            // Arrange
            var material = InitDB();

            // Act
            var result = await _materialRepository.GetMaterialByIdAsync(material.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(material.Id, result.Id);
            Assert.Equal(material.Name, result.Name);
            Assert.Equal(material.Description, result.Description);
            Assert.Equal(material.Unit, result.Unit);
            Assert.Equal(material.QuantityPerUnit, result.QuantityPerUnit);
            Assert.Equal(material.Image, result.Image);
        }

        [Fact]
        public async Task GetMaterialByIdAsync_WithNonExistingMaterialId_ShouldReturnNull()
        {
            // Act
            var result = await _materialRepository.GetMaterialByIdAsync(999); // ID that does not exist

            // Assert
            Assert.Null(result);
        }

        private Material InitDB()
        {
            var createMaterialRequest = new CreateMaterialRequest
            (
                Name: "Material 1",
                Description: "Description 1",
                Unit: "Unit 1",
                QuantityPerUnit: 10,
                Image: "Image 1",
                QuantityInStock: 10
            );
            var material = Material.Create(createMaterialRequest);
            _context.Materials.Add(material);
            _context.SaveChanges();

            return material;
        }
    }
}
