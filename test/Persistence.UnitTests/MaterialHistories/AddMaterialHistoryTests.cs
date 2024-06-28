using Contract.Services.Material.Create;
using Contract.Services.MaterialHistory.Create;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.MaterialHistories
{
    public class AddMaterialHistoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly MaterialHistoryRepository _materialHistoryRepository;

        public AddMaterialHistoryTests()
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
        public async Task AddMaterialHistory_ShouldAddNewMaterialHistory()
        {
            // Arrange
            var CreateMaterialHistoryRequest = new CreateMaterialHistoryRequest
                (
                    MaterialId: 1,
                    Quantity: 10,
                    Price: 10,
                    Description: "Description 1",
                    ImportDate: "06/06/2024"
                );

            // Act
            var materialHistory = MaterialHistory.Create(CreateMaterialHistoryRequest);
            _materialHistoryRepository.AddMaterialHistory(materialHistory);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.MaterialHistories.FindAsync(materialHistory.Id);
            Assert.NotNull(result);
            Assert.Equal(materialHistory.Id, result.Id);
        }
    }
}
