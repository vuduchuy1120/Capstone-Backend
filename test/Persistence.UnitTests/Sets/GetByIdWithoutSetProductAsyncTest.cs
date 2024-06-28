using Application.Abstractions.Data;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.SharedDto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Sets;

public class GetByIdWithoutSetProductAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetRepository _setRepository;

    public GetByIdWithoutSetProductAsyncTest()
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
    public async Task GetByIdWithoutSetProductAsync_ExistingSetWithoutProducts_ShouldReturnSet()
    {
        // Arrange
        var set = Set.Create(new CreateSetCommand(
            new CreateSetRequest("Code", "Name", "Description", "Image", null), "CreatedBy"));
        _context.Sets.Add(set);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setRepository.GetByIdWithoutSetProductAsync(set.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(set.Id, result.Id);
        Assert.Null(result.SetProducts);
    }

    [Fact]
    public async Task GetByIdWithoutSetProductAsync_NonExistingSet_ShouldReturnNull()
    {
        // Act
        var result = await _setRepository.GetByIdWithoutSetProductAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdWithoutSetProductAsync_ExistingSetWithProducts_ShouldReturnSetWithoutProducts()
    {
        // Arrange
        var setProductsRequest = new List<SetProductRequest>()
            {
                new SetProductRequest(Guid.NewGuid(), 5),
                new SetProductRequest(Guid.NewGuid(), 5)
            };
        var set = Set.Create(new CreateSetCommand(
            new CreateSetRequest("Code", "Name", "Description", "Image", setProductsRequest), "CreatedBy"));
        _context.Sets.Add(set);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setRepository.GetByIdWithoutSetProductAsync(set.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(set.Id, result.Id);
        Assert.Null(result.SetProducts);
    }
}
