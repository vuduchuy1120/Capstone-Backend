using Application.Abstractions.Data;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.SharedDto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Sets;

public class AddSetTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetRepository _setRepository;
    public AddSetTest()
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
    public async Task AddSet_WithoutSetProducts_ShouldAddSetToContext()
    {
        var createSetRequest = new CreateSetRequest("Code", "Name", "Description", "Image", null);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");
        var set = Set.Create(createSetCommand);

        _setRepository.Add(set);
        await _context.SaveChangesAsync();

        var addedSet = await _context.Sets.FirstOrDefaultAsync();
        Assert.NotNull(addedSet);
        Assert.Equal(set.Id, addedSet.Id);
    }

    [Fact]
    public async Task AddSet_WithSetProducts_ShouldAddSetToContextWithSetProducts()
    {
        var setProductsRequest = new List<SetProductRequest>()
        {
            new SetProductRequest(Guid.NewGuid(), 5),
            new SetProductRequest(Guid.NewGuid(), 5)
        };
        var createSetRequest = new CreateSetRequest("Code", "Name", "Description", "Image", setProductsRequest);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");
        var set = Set.Create(createSetCommand);

        _setRepository.Add(set);
        await _context.SaveChangesAsync();

        var addedSet = await _context.Sets.Include(s => s.SetProducts).FirstOrDefaultAsync();
        Assert.NotNull(addedSet);
        Assert.Equal(set.Id, addedSet.Id);
        Assert.NotNull(addedSet.SetProducts);
        Assert.Equal(2, addedSet.SetProducts.Count);
    }
}
