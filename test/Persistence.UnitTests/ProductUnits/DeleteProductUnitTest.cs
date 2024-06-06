using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductUnits;

public class DeleteProductUnitTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductUnitRepository _productUnitRepository;

    public DeleteProductUnitTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productUnitRepository = new ProductUnitRepository(_context);
    }
    [Fact]
    public async Task DeleteProductUnit_Success_Should_RemoveProductUnitFromDb()
    {
        var productId = Guid.NewGuid();
        var subProductId = Guid.NewGuid();
        var quantityPerUnit = 5;
        var productUnit = ProductUnit.Create(productId, subProductId, quantityPerUnit);

        _productUnitRepository.Add(productUnit);
        await _context.SaveChangesAsync();

        _productUnitRepository.Delete(productUnit);
        await _context.SaveChangesAsync();

        var units = await _context.ProductUnits.ToListAsync();
        Assert.Empty(units);
    }

    [Fact]
    public async Task DeleteProductUnit_NotInDb_Should_ThrowDbUpdateConcurrencyException()
    {
        var productId = Guid.NewGuid();
        var subProductId = Guid.NewGuid();
        var quantityPerUnit = 5;
        var productUnit = ProductUnit.Create(productId, subProductId, quantityPerUnit);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
        {
            _productUnitRepository.Delete(productUnit);
            await _context.SaveChangesAsync();
        });

        var units = await _context.ProductUnits.ToListAsync();
        Assert.Empty(units);
    }

    [Fact]
    public async Task DeleteProductUnit_Null_Should_ThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _productUnitRepository.Delete(null);
            await _context.SaveChangesAsync();
        });
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
