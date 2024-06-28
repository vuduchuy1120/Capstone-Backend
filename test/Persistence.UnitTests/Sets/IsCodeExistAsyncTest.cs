using Application.Abstractions.Data;
using Contract.Services.Set.CreateSet;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Sets;

public class IsCodeExistAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetRepository _setRepository;

    public IsCodeExistAsyncTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _setRepository = new SetRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task IsCodeExistAsync_SetWithSpecifiedCodeExists_ShouldReturnTrue()
    {
        // Arrange
        var set = Set.Create(new CreateSetCommand(
            new CreateSetRequest("Code123", "Name", "Description", "Image", null), "CreatedBy"));
        _context.Sets.Add(set);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setRepository.IsCodeExistAsync("Code123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsCodeExistAsync_SetWithSpecifiedCodeDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _setRepository.IsCodeExistAsync("NonExistingCode");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsCodeExistAsync_EmptyDatabase_ShouldReturnFalse()
    {
        // Act
        var result = await _setRepository.IsCodeExistAsync("AnyCode");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsCodeExistAsync_SetWithSpecifiedCodeExists_CaseInsensitive_ShouldReturnFalse()
    {
        // Arrange
        var set = Set.Create(new CreateSetCommand(
            new CreateSetRequest("Code123", "Name", "Description", "Image", null), "CreatedBy"));
        _context.Sets.Add(set);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setRepository.IsCodeExistAsync("code123");

        // Assert
        Assert.False(result);
    }
}
