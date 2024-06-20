using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Persistence.UnitTests.Phases
{
    public class GetPhasesTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IPhaseRepository _phaseRepository;

        public GetPhasesTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _phaseRepository = new PhaseRepository(_context);
        }

        [Fact]
        public async Task GetPhases_ShouldReturnAllPhases()
        {
            // Arrange
            var phases = new List<Phase>
            {
                new Phase { Id = Guid.NewGuid(), Name = "Phase 1", Description = "Description of Phase 1" },
                new Phase { Id = Guid.NewGuid(), Name = "Phase 2", Description = "Description of Phase 2" },
                new Phase { Id = Guid.NewGuid(), Name = "Phase 3", Description = "Description of Phase 3" }
            };

            await _context.Phases.AddRangeAsync(phases);
            await _context.SaveChangesAsync();

            // Act
            var retrievedPhases = await _phaseRepository.GetPhases();

            // Assert
            Assert.NotNull(retrievedPhases);
            Assert.Equal(3, retrievedPhases.Count);

            foreach (var phase in phases)
            {
                var retrievedPhase = retrievedPhases.FirstOrDefault(p => p.Id == phase.Id);
                Assert.NotNull(retrievedPhase);
                Assert.Equal(phase.Name, retrievedPhase.Name);
                Assert.Equal(phase.Description, retrievedPhase.Description);
            }
        }

        [Fact]
        public async Task GetPhases_NoPhasesInDb_ShouldReturnEmptyList()
        {
            // Arrange - no phases added to the database

            // Act
            var retrievedPhases = await _phaseRepository.GetPhases();

            // Assert
            Assert.NotNull(retrievedPhases);
            Assert.Empty(retrievedPhases);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
