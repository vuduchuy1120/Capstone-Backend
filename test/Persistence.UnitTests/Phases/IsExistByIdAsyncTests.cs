using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Phases
{
    public class IsExistByIdAsyncTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IPhaseRepository _phaseRepository;

        public IsExistByIdAsyncTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _phaseRepository = new PhaseRepository(_context);
        }

        [Fact]
        public async Task IsExistById_ExistingId_ShouldReturnTrue()
        {
            // Arrange
            var phaseId = Guid.NewGuid();
            var phase = new Phase
            {
                Id = phaseId,
                Name = "Phase " + phaseId // Bổ sung thuộc tính bắt buộc Name
            };
            await _context.Phases.AddAsync(phase);
            await _context.SaveChangesAsync();

            // Act
            var result = await _phaseRepository.IsExistById(phaseId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsExistById_NonExistingId_ShouldReturnFalse()
        {
            // Arrange
            var phaseId = Guid.NewGuid();

            // Act
            var result = await _phaseRepository.IsExistById(phaseId);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
