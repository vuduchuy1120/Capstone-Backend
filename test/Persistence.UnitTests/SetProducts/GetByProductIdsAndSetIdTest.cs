using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.SetProducts;

public class GetByProductIdsAndSetIdTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetProductRepository _setProductRepository;

    public GetByProductIdsAndSetIdTest()
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
    public async Task GetByProductIdsAndSetId_ShouldReturnMatchingSetProducts()
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

        // Act
        var result = await _setProductRepository.GetByProductIdsAndSetId(productIds, setId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, sp => Assert.Equal(setId, sp.SetId));
        Assert.All(result, sp => Assert.Contains(sp.ProductId, productIds));
    }

    [Fact]
    public async Task GetByProductIdsAndSetId_NoMatchingSetProducts_ShouldReturnEmptyList()
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

        var productIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await _setProductRepository.GetByProductIdsAndSetId(productIds, setId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
