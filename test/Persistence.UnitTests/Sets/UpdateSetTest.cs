using Application.Abstractions.Data;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.UpdateSet;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Sets;
public class UpdateSetTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetRepository _setRepository;

    public UpdateSetTest()
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
    public async Task Update_ExistingSet_ShouldUpdateSet()
    {
        var createSetRequest = new CreateSetRequest("Code", "Name", "Description", "Image", null);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");
        var set = Set.Create(createSetCommand);
        _context.Sets.Add(set);
        await _context.SaveChangesAsync();

        var updateSetRequest = new UpdateSetRequest(set.Id, "UpdatedCode", "UpdatedName", "UpdatedDescription",
            "http://example.com/updated.jpg", null, null, null);
        set.Update(updateSetRequest, "updatedBy");
        _setRepository.Update(set);
        await _context.SaveChangesAsync();

        var updatedSet = await _context.Sets.FindAsync(set.Id);

        Assert.NotNull(updatedSet);
        Assert.Equal(updatedSet.Id, set.Id);
        Assert.Equal("UpdatedName", updatedSet.Name);
        Assert.Equal("UpdatedDescription", updatedSet.Description);
        Assert.Equal("UpdatedCode", updatedSet.Code);
        Assert.Equal("http://example.com/updated.jpg", updatedSet.ImageUrl);
        Assert.Equal("updatedBy", updatedSet.UpdatedBy);
    }

    [Fact]
    public async Task Update_NonExistingSet_ShouldThrowException()
    {
        var createSetRequest = new CreateSetRequest("Code", "Name", "Description", "Image", null);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");
        var nonExistingSet = Set.Create(createSetCommand);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
        {
            _setRepository.Update(nonExistingSet);
            await _context.SaveChangesAsync();
        });
    }

}
