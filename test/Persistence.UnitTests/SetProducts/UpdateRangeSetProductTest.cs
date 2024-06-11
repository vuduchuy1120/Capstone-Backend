using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.SetProducts;

public class UpdateRangeSetProductTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetProductRepository _setProductRepository;

    public UpdateRangeSetProductTest()
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
    public async Task UpdateRange_ShouldUpdateMultipleSetProductsInContext()
    {
        // Arrange
        var setProducts = new List<SetProduct>
            {
                SetProduct.Create(Guid.NewGuid(), Guid.NewGuid(), 5),
                SetProduct.Create(Guid.NewGuid(), Guid.NewGuid(), 10)
            };

        _context.SetProducts.AddRange(setProducts);
        await _context.SaveChangesAsync();

        // Modify the set products
        setProducts[0].Update(15);
        setProducts[1].Update(20);

        // Act
        _setProductRepository.UpdateRange(setProducts);
        await _context.SaveChangesAsync();

        // Assert
        var updatedSetProducts = await _context.SetProducts.ToListAsync();
        Assert.Equal(2, updatedSetProducts.Count);
        Assert.Equal(15, updatedSetProducts[0].Quantity);
        Assert.Equal(20, updatedSetProducts[1].Quantity);
    }

    [Fact]
    public async Task UpdateRange_ShouldHandleEmptyList()
    {
        // Arrange
        var setProducts = new List<SetProduct>();

        // Act
        _setProductRepository.UpdateRange(setProducts);
        await _context.SaveChangesAsync();

        // Assert
        var remainingSetProducts = await _context.SetProducts.ToListAsync();
        Assert.Empty(remainingSetProducts);
    }

    [Fact]
    public void UpdateRange_ShouldThrowException_WhenNullListIsPassed()
    {
        // Arrange
        List<SetProduct> setProducts = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _setProductRepository.UpdateRange(setProducts));
    }
}
