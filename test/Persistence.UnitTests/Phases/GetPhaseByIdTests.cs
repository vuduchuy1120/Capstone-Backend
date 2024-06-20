using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Phases
{
    public class GetPhaseByIdTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IPhaseRepository _phaseRepository;

        public GetPhaseByIdTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _phaseRepository = new PhaseRepository(_context);
        }

        [Fact]
        public async Task GetPhaseById_ExistingId_ShouldReturnCorrectPhase()
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

            // Act
            var retrievedPhase = await _phaseRepository.GetPhaseById(phase.Id);

            // Assert
            Assert.NotNull(retrievedPhase);
            Assert.Equal(phase.Id, retrievedPhase.Id);
            Assert.Equal(phase.Name, retrievedPhase.Name);
            Assert.Equal(phase.Description, retrievedPhase.Description);
        }

        [Fact]
        public async Task GetPhaseById_NonExistentId_ShouldReturnNull()
        {
            // Arrange - no need to add any phase

            // Act
            var retrievedPhase = await _phaseRepository.GetPhaseById(Guid.NewGuid());

            // Assert
            Assert.Null(retrievedPhase);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
