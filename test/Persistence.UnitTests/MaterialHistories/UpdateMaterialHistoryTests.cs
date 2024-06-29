using Contract.Services.MaterialHistory.Create;
using Contract.Services.MaterialHistory.Update;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.MaterialHistories
{
    public class UpdateMaterialHistoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialHistoryRepository _materialHistoryRepository;

        public UpdateMaterialHistoryTests()
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
        public async Task UpdateMaterialHistory_ShouldUpdateExistingMaterialHistory()
        {
            // Arrange
            var materialHistory = InitDB();
            var updateMaterialHistoryRequest = new UpdateMaterialHistoryRequest
            (
                Id: materialHistory.Id,
                MaterialId: Guid.NewGuid(),
                Quantity: 20,
                Price: 10,
                Description: "Description 1",
                ImportDate: "06/06/2024"
            );
            materialHistory.Update(updateMaterialHistoryRequest);
            // Act
            _materialHistoryRepository.UpdateMaterialHistory(materialHistory);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.MaterialHistories.FindAsync(materialHistory.Id);
            Assert.NotNull(result);
            Assert.Equal(20, result.Quantity);
        }

        private MaterialHistory InitDB()
        {
            var createMaterialHistoryRequest = new CreateMaterialHistoryRequest
            (
                MaterialId: Guid.NewGuid(),
                Quantity: 10,
                Price: 10,
                Description: "Description 1",
                ImportDate: "06/06/2024"
            );

            var materialHistory = MaterialHistory.Create(createMaterialHistoryRequest);
            _context.MaterialHistories.Add(materialHistory);
            _context.SaveChanges();

            return materialHistory;
        }
    }
}
