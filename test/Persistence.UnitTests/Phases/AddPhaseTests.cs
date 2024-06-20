using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Phases
{
    public class AddPhaseTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IPhaseRepository _phaseRepository;

        public AddPhaseTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _phaseRepository = new PhaseRepository(_context);
        }

        [Fact]
        public async Task AddPhase_Success_ShouldHaveNewPhaseInDb()
        {
            // Arrange
            var phase = new Phase
            {
                Id = Guid.NewGuid(),
                Name = "Phase 1",
                Description = "Description of Phase 1"
            };

            // Act
            _phaseRepository.AddPhase(phase);
            await _context.SaveChangesAsync();

            // Assert
            Assert.Single(_context.Phases);
            var savedPhase = await _context.Phases.FirstOrDefaultAsync(p => p.Id == phase.Id);

            Assert.NotNull(savedPhase);
            Assert.Equal(phase.Id, savedPhase.Id);
        }

        [Fact]
        public async Task AddPhase_IdExisted_Error_ShouldNotHaveNewPhaseToDb()
        {
            // Arrange
            var phase = new Phase
            {
                Id = Guid.NewGuid(),
                Name = "Phase 1",
                Description = "Description of Phase 1"
            };

            _phaseRepository.AddPhase(phase);
            await _context.SaveChangesAsync();

            // Act & Assert
            var duplicatePhase = new Phase
            {
                Id = phase.Id,
                Name = "Phase 2",
                Description = "Description of Phase 2"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                _phaseRepository.AddPhase(duplicatePhase);
                await _context.SaveChangesAsync();
            });

            Assert.Single(_context.Phases);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
