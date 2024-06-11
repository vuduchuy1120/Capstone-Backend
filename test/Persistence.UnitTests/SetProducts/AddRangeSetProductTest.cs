using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.SetProducts;

public class AddRangeSetProductTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetProductRepository _setProductRepository;

    public AddRangeSetProductTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _setProductRepository = new SetProductRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task AddRange_ShouldAddMultipleSetProductsToContext()
    {
        // Arrange
        var setProducts = new List<SetProduct>
            {
                SetProduct.Create(Guid.NewGuid(), Guid.NewGuid(), 5),
                SetProduct.Create(Guid.NewGuid(), Guid.NewGuid(), 10)
            };

        // Act
        _setProductRepository.AddRange(setProducts);
        await _context.SaveChangesAsync();

        // Assert
        var addedSetProducts = await _context.SetProducts.ToListAsync();
        Assert.Equal(setProducts.Count, addedSetProducts.Count);
    }

    [Fact]
    public async Task AddRange_ShouldHandleEmptyList()
    {
        // Arrange
        var setProducts = new List<SetProduct>();

        // Act
        _setProductRepository.AddRange(setProducts);
        await _context.SaveChangesAsync();

        // Assert
        var addedSetProducts = await _context.SetProducts.ToListAsync();
        Assert.Empty(addedSetProducts);
    }

    [Fact]
    public void AddRange_ShouldThrowException_WhenNullListIsPassed()
    {
        // Arrange
        List<SetProduct> setProducts = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _setProductRepository.AddRange(setProducts));
    }
}
