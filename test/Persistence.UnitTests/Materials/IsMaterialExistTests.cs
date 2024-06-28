using Contract.Services.Material.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.Materials
{
    public class IsMaterialExistTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialRepository _materialRepository;

        public IsMaterialExistTests()
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
        public async Task IsMaterialExist_WithExistingMaterialId_ShouldReturnTrue()
        {
            // Arrange
            var material = InitDB();

            // Act
            var result = await _materialRepository.IsMaterialExist(material.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsMaterialExist_WithNonExistingMaterialId_ShouldReturnFalse()
        {
            // Act
            var result = await _materialRepository.IsMaterialExist(999); // ID that does not exist

            // Assert
            Assert.False(result);
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
