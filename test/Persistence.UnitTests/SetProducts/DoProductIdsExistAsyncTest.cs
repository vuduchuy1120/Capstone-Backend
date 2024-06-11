using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.SetProducts;

public class DoProductIdsExistAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetProductRepository _setProductRepository;

    public DoProductIdsExistAsyncTest()
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
    public async Task DoProductIdsExistAsync_AllProductIdsExist_ShouldReturnTrue()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var productIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var setProducts = productIds.Select(productId => SetProduct.Create(setId, productId, 1)).ToList();
        _context.SetProducts.AddRange(setProducts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setProductRepository.DoProductIdsExistAsync(productIds, setId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoProductIdsExistAsync_SomeProductIdsDoNotExist_ShouldReturnFalse()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var existingProductId = Guid.NewGuid();
        var missingProductId = Guid.NewGuid();

        var setProducts = new List<SetProduct> { SetProduct.Create(setId, existingProductId, 1) };
        _context.SetProducts.AddRange(setProducts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setProductRepository.DoProductIdsExistAsync(new List<Guid> { existingProductId, missingProductId }, setId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DoProductIdsExistAsync_NoneProductIdsExist_ShouldReturnFalse()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var productIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await _setProductRepository.DoProductIdsExistAsync(productIds, setId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DoProductIdsExistAsync_EmptyProductIdsList_ShouldReturnTrue()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var productIds = new List<Guid>();

        // Act
        var result = await _setProductRepository.DoProductIdsExistAsync(productIds, setId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DoProductIdsExistAsync_SetIdDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var productIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        var setProducts = productIds.Select(productId => SetProduct.Create(Guid.NewGuid(), productId, 1)).ToList();
        _context.SetProducts.AddRange(setProducts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setProductRepository.DoProductIdsExistAsync(productIds, setId);

        // Assert
        Assert.False(result);
    }
}
