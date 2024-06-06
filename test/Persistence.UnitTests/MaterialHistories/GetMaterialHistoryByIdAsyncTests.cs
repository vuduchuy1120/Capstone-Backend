using Contract.Services.MaterialHistory.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.MaterialHistories
{
    public class GetMaterialHistoryByIdAsyncTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialHistoryRepository _materialHistoryRepository;

        public GetMaterialHistoryByIdAsyncTests()
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
        public async Task GetMaterialHistoryByIdAsync_WithExistingMaterialHistoryId_ShouldReturnMaterialHistory()
        {
            // Arrange
            var materialHistory = InitDB();

            // Act
            var result = await _materialHistoryRepository.GetMaterialHistoryByIdAsync(materialHistory.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(materialHistory.Id, result.Id);
        }

        [Fact]
        public async Task GetMaterialHistoryByIdAsync_WithNonExistingMaterialHistoryId_ShouldReturnNull()
        {
            // Act
            var result = await _materialHistoryRepository.GetMaterialHistoryByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
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
