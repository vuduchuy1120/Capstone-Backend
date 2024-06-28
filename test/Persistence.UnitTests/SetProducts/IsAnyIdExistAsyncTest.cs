using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Persistence.Repositories;

namespace Persistence.UnitTests.SetProducts;

public class IsAnyIdExistAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetProductRepository _setProductRepository;

    public IsAnyIdExistAsyncTest()
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
    public async Task IsAnyIdExistAsync_AllProductIdsExist_ShouldReturnTrue()
    {
        // Arrange
        var setId = Guid.NewGuid();

        var setProducts = new List<SetProduct>
        {
            SetProduct.Create(setId, Guid.NewGuid(), 5),
            SetProduct.Create(setId, Guid.NewGuid(), 10)
        };

        _setProductRepository.AddRange(setProducts);
        await _context.SaveChangesAsync();

        var productIds = setProducts.ConvertAll(s => s.ProductId).ToList();

        //Act
        var result = await _setProductRepository.IsAnyIdExistAsync(productIds, setId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAnyIdExistAsync_SomeProductIdsExist_ShouldReturnTrue()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var setProducts = new List<SetProduct>
        {
            SetProduct.Create(setId, productId1, 5),
            SetProduct.Create(setId, Guid.NewGuid(), 5),
            SetProduct.Create(setId, productId2, 5),
            SetProduct.Create(setId, Guid.NewGuid(), 10)
        };

        _setProductRepository.AddRange(setProducts);
        await _context.SaveChangesAsync();

        var productIds = new List<Guid> { productId1, productId2 };

        //Act
        var result = await _setProductRepository.IsAnyIdExistAsync(productIds, setId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsAnyIdExistAsync_NoneProductIdsExist_ShouldReturnFalse()
    {
        // Arrange
        var setId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var setProducts = new List<SetProduct>
        {
            SetProduct.Create(setId, Guid.NewGuid(), 5),
            SetProduct.Create(setId, Guid.NewGuid(), 10)
        };

        _setProductRepository.AddRange(setProducts);
        await _context.SaveChangesAsync();

        var productIds = new List<Guid> { productId1, productId2 };

        //Act
        var result = await _setProductRepository.IsAnyIdExistAsync(productIds, setId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsAnyIdExistAsync_EmptyProductIdsList_ShouldReturnFalse()
    {
        // Arrange
        var setId = Guid.NewGuid();

        var productIds = new List<Guid> { };

        //Act
        var result = await _setProductRepository.IsAnyIdExistAsync(productIds, setId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsAnyIdExistAsync_SetIdDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var setId = Guid.NewGuid();

        var setProducts = new List<SetProduct>
        {
            SetProduct.Create(setId, Guid.NewGuid(), 5),
            SetProduct.Create(setId, Guid.NewGuid(), 10)
        };

        _setProductRepository.AddRange(setProducts);
        await _context.SaveChangesAsync();

        var productIds = setProducts.ConvertAll(s => s.ProductId).ToList();

        //Act
        var result = await _setProductRepository.IsAnyIdExistAsync(productIds, Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}
