using Contract.Services.Material.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.Materials
{
    public class GetMaterialsByNameAsyncTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialRepository _materialRepository;

        public GetMaterialsByNameAsyncTests()
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
        public async Task GetMaterialsByNameAsync_WithExistingMaterialName_ShouldReturnMaterials()
        {
            // Arrange
            InitDB();

            // Act
            var result = await _materialRepository.GetMaterialsByNameAsync("Material");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, item => Assert.Contains("Material", item.Name));
        }

        [Fact]
        public async Task GetMaterialsByNameAsync_WithNonExistingMaterialName_ShouldReturnEmptyList()
        {
            // Arrange
            InitDB();

            // Act
            var result = await _materialRepository.GetMaterialsByNameAsync("NonExistingName");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        private void InitDB()
        {
            var materials = new List<CreateMaterialRequest>
            {
                new CreateMaterialRequest("Material 1", "Description 1", "Unit 1", 10, "Image 1"),
                new CreateMaterialRequest("Material 2", "Description 2", "Unit 2", 20, "Image 2"),
                new CreateMaterialRequest("Different Material", "Description 3", "Unit 3", 30, "Image 3")
            };

            foreach (var request in materials)
            {
                var material = Material.Create(request);
                _context.Materials.Add(material);
            }

            _context.SaveChanges();
        }
    }
}
