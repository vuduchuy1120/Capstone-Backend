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
    public class GetMaterialUnitsAsyncTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialRepository _materialRepository;

        public GetMaterialUnitsAsyncTests()
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
        public async Task GetMaterialUnitsAsync_ShouldReturnDistinctUnits()
        {
            // Arrange
            InitDB();

            // Act
            var result = await _materialRepository.GetMaterialUnitsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Unit 1", result);
            Assert.Contains("Unit 2", result);
            Assert.Equal(2, result.Count); // Expecting only 2 distinct units
        }

        private void InitDB()
        {
            var createMaterialRequest = new CreateMaterialRequest
           (
               Name: "Material 1",
               Description: "Description 1",
               Unit: "Unit 1",
               QuantityPerUnit: 10,
               Image: "Image 1"
           );
            var material = Material.Create(createMaterialRequest);
            _context.Materials.Add(material);
            _context.SaveChanges();

            var createMaterialRequest2 = new CreateMaterialRequest
          (
              Name: "Material 1",
              Description: "Description 1",
              Unit: "Unit 2",
              QuantityPerUnit: 10,
              Image: "Image 1"
          );
            var material2 = Material.Create(createMaterialRequest2);
            _context.Materials.Add(material2);
            _context.SaveChanges();

            var createMaterialRequest3 = new CreateMaterialRequest
          (
              Name: "Material 1",
              Description: "Description 1",
              Unit: "Unit 2",
              QuantityPerUnit: 10,
              Image: "Image 1"
          );
            var material3 = Material.Create(createMaterialRequest3);
            _context.Materials.Add(material3);
            _context.SaveChanges();

        }
    }
}
