using Application.Abstractions.Data;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.GetSets;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Sets;

public class SearchSetAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetRepository _setRepository;

    public SearchSetAsyncTest()
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
    public async Task SearchSetAsync_WithoutSearchTerm_ShouldReturnAllSets()
    {
        // Arrange
        var sets = new List<Set>
        {
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code1", "Name1", "Description1", "Image1", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code2", "Name2", "Description2", "Image2", null), "CreatedBy"))
        };
        _context.Sets.AddRange(sets);
        await _context.SaveChangesAsync();

        var query = new GetSetsQuery("", 1, 10);

        // Act
        var (resultSets, totalPages) = await _setRepository.SearchSetAsync(query);

        // Assert
        Assert.Equal(sets.Count, resultSets.Count);
        Assert.Equal(1, totalPages);
    }

    [Fact]
    public async Task SearchSetAsync_WithSearchTerm_ShouldFilterSets()
    {
        // Arrange
        var sets = new List<Set>
        {
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code1", "MatchingName", "Description1", "Image1", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code2", "NonMatchingName", "Description2", "Image2", null), "CreatedBy"))
        };
        _context.Sets.AddRange(sets);
        await _context.SaveChangesAsync();

        var query = new GetSetsQuery("Matching", 1, 10);

        // Act
        var (resultSets, totalPages) = await _setRepository.SearchSetAsync(query);

        // Assert
        Assert.Equal(2, resultSets.Count());
        Assert.Equal("MatchingName", resultSets.First().Name);
        Assert.Equal(1, totalPages);
    }

    [Fact]
    public async Task SearchSetAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var sets = new List<Set>
        {
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code1", "Name1", "Description1", "Image1", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code2", "Name2", "Description2", "Image2", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code3", "Name3", "Description3", "Image3", null), "CreatedBy"))
        };
        _context.Sets.AddRange(sets);
        await _context.SaveChangesAsync();

        var query = new GetSetsQuery ("", 2, 2);

        // Act
        var (resultSets, totalPages) = await _setRepository.SearchSetAsync(query);

        // Assert
        Assert.Single(resultSets); // Only one set on the second page
        Assert.Equal("Code3", resultSets.First().Code); // The second set's Code should be "Code2"
        Assert.Equal(2, totalPages); // 3 sets, 2 per page, should result in 2 pages
    }

    [Fact]
    public async Task SearchSetAsync_WithNoMatchingResults_ShouldReturnEmpty()
    {
        // Arrange
        var sets = new List<Set>
        {
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code1", "Name1", "Description1", "Image1", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code2", "Name2", "Description2", "Image2", null), "CreatedBy"))
        };
        _context.Sets.AddRange(sets);
        await _context.SaveChangesAsync();

        var query = new GetSetsQuery ("NonExistent", 1, 10);

        // Act
        var (resultSets, totalPages) = await _setRepository.SearchSetAsync(query);

        // Assert
        Assert.Empty(resultSets);
        Assert.Equal(0, totalPages);
    }

    [Fact]
    public async Task SearchSetAsync_WithPartialResults_ShouldReturnCorrectPage()
    {
        // Arrange
        var sets = new List<Set>
        {
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code1", "Name1", "Description1", "Image1", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code2", "Name2", "Description2", "Image2", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code3", "Name3", "Description3", "Image3", null), "CreatedBy")),
            Set.Create(new CreateSetCommand(new CreateSetRequest("Code4", "Name4", "Description4", "Image4", null), "CreatedBy"))
        };
        _context.Sets.AddRange(sets);
        await _context.SaveChangesAsync();

        var query = new GetSetsQuery ("", 2, 2);

        // Act
        var (resultSets, totalPages) = await _setRepository.SearchSetAsync(query);

        // Assert
        Assert.Equal(2, resultSets.Count); // Two sets on the second page
        Assert.Equal("Code3", resultSets.First().Code); // The first set on the second page
        Assert.Equal("Code4", resultSets.Last().Code); // The second set on the second page
        Assert.Equal(2, totalPages); // 4 sets, 2 per page, should result in 2 pages
    }
}
