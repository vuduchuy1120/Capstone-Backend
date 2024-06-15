using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.SetProducts;

public class DeleteRangeSetProductTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetProductRepository _setProductRepository;

    public DeleteRangeSetProductTest()
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
    public async Task DeleteRange_ShouldRemoveMultipleSetProductsFromContext()
    {
        // Arrange
        var setProducts = new List<SetProduct>
            {
                SetProduct.Create(Guid.NewGuid(), Guid.NewGuid(), 5),
                SetProduct.Create(Guid.NewGuid(), Guid.NewGuid(), 10)
            };

        _context.SetProducts.AddRange(setProducts);
        await _context.SaveChangesAsync();

        // Act
        _setProductRepository.DeleteRange(setProducts);
        await _context.SaveChangesAsync();

        // Assert
        var remainingSetProducts = await _context.SetProducts.ToListAsync();
        Assert.Empty(remainingSetProducts);
    }

    [Fact]
    public async Task DeleteRange_ShouldHandleEmptyList()
    {
        // Arrange
        var setProducts = new List<SetProduct>();

        // Act
        _setProductRepository.DeleteRange(setProducts);
        await _context.SaveChangesAsync();

        // Assert
        var remainingSetProducts = await _context.SetProducts.ToListAsync();
        Assert.Empty(remainingSetProducts);
    }

    [Fact]
    public void DeleteRange_ShouldThrowException_WhenNullListIsPassed()
    {
        // Arrange
        List<SetProduct> setProducts = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _setProductRepository.DeleteRange(setProducts));
    }
}
