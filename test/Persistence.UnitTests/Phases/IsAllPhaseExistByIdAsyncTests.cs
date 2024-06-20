using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Phases
{
    public class IsAllPhaseExistByIdAsyncTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IPhaseRepository _phaseRepository;

        public IsAllPhaseExistByIdAsyncTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _phaseRepository = new PhaseRepository(_context);
        }

        [Fact]
        public async Task IsAllPhaseExistByIdAsync_AllPhasesExist_ShouldReturnTrue()
        {
            // Arrange
            var phaseIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            var phases = phaseIds.Select(id => new Phase
            {
                Id = id,
                Name = "Phase " + id // Bổ sung thuộc tính bắt buộc Name
            }).ToList();

            await _context.Phases.AddRangeAsync(phases);
            await _context.SaveChangesAsync();

            // Act
            var result = await _phaseRepository.IsAllPhaseExistByIdAsync(phaseIds);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsAllPhaseExistByIdAsync_NotAllPhasesExist_ShouldReturnFalse()
        {
            // Arrange
            var existingPhaseId = Guid.NewGuid();
            var nonExistingPhaseId = Guid.NewGuid();

            var phaseIds = new List<Guid>
            {
                existingPhaseId,
                nonExistingPhaseId
            };

            var phase = new Phase
            {
                Id = existingPhaseId,
                Name = "Phase " + existingPhaseId // Bổ sung thuộc tính bắt buộc Name
            };
            await _context.Phases.AddAsync(phase);
            await _context.SaveChangesAsync();

            // Act
            var result = await _phaseRepository.IsAllPhaseExistByIdAsync(phaseIds);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
