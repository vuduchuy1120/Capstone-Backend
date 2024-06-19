using Application.Abstractions.Data;
using Contract.Services.Phase.Updates;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Phases
{
    public class UpdatePhaseTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IPhaseRepository _phaseRepository;

        public UpdatePhaseTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _phaseRepository = new PhaseRepository(_context);
        }

        [Fact]
        public async Task UpdatePhase_Success_ShouldUpdateExistingPhase()
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
            var updatedPhase = new UpdatePhaseRequest
            (
                Id: phase.Id,
                Name: "Updated Phase",
                Description: "Updated description"
            );
            phase.Update(updatedPhase);
            _phaseRepository.UpdatePhase(phase);
            await _context.SaveChangesAsync();

            // Assert
            var savedPhase = await _phaseRepository.GetPhaseById(phase.Id);
            Assert.NotNull(savedPhase);
            Assert.Equal(updatedPhase.Name, savedPhase.Name);
            Assert.Equal(updatedPhase.Description, savedPhase.Description);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
