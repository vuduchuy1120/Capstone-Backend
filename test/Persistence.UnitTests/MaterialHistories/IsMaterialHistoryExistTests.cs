using Contract.Services.MaterialHistory.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.MaterialHistories
{
    public class IsMaterialHistoryExistTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialHistoryRepository _materialHistoryRepository;

        public IsMaterialHistoryExistTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _materialHistoryRepository = new MaterialHistoryRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task IsMaterialHistoryExist_WithExistingMaterialHistoryId_ShouldReturnTrue()
        {
            // Arrange
            var materialHistory = InitDB();

            // Act
            var result = await _materialHistoryRepository.IsMaterialHistoryExist(materialHistory.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsMaterialHistoryExist_WithNonExistingMaterialHistoryId_ShouldReturnFalse()
        {
            // Act
            var result = await _materialHistoryRepository.IsMaterialHistoryExist(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        private MaterialHistory InitDB()
        {
            var createMaterialHistoryRequest = new CreateMaterialHistoryRequest
            (
                MaterialId: 1,
                Quantity: 10,
                QuantityPerUnit: 10,
                Price: 10,
                Description: "Description 1",
                QuantityInStock: 10,
                ImportDate: "06/06/2024"
            );

            var materialHistory = MaterialHistory.Create(createMaterialHistoryRequest);
            _context.MaterialHistories.Add(materialHistory);
            _context.SaveChanges();

            return materialHistory;
        }
    }
}
