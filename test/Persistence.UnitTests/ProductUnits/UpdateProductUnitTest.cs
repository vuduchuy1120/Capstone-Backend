using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductUnits;

public class UpdateProductUnitTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductUnitRepository _productUnitRepository;

    public UpdateProductUnitTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productUnitRepository = new ProductUnitRepository(_context);
    }
    [Fact]
    public async Task UpdateProductUnit_Success_Should_UpdateProductUnitInDb()
    {
        var productId = Guid.NewGuid();
        var subProductId = Guid.NewGuid();
        var quantityPerUnit = 5;
        var productUnit = ProductUnit.Create(productId, subProductId, quantityPerUnit);

        _productUnitRepository.Add(productUnit);
        await _context.SaveChangesAsync();

        var updatedQuantityPerUnit = 10;
        productUnit.Update(updatedQuantityPerUnit);
        _productUnitRepository.Update(productUnit);
        await _context.SaveChangesAsync();

        var updatedProductUnit = await _context.ProductUnits
            .FirstOrDefaultAsync(pu => pu.ProductId == productId && pu.SubProductId == subProductId);
        Assert.NotNull(updatedProductUnit);
        Assert.Equal(updatedQuantityPerUnit, updatedProductUnit.QuantityPerUnit);
    }

    [Fact]
    public async Task UpdateProductUnit_NotInDb_Should_ThrowDbUpdateConcurrencyException()
    {
        var productId = Guid.NewGuid();
        var subProductId = Guid.NewGuid();
        var quantityPerUnit = 5;
        var productUnit = ProductUnit.Create(productId, subProductId, quantityPerUnit);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
        {
            _productUnitRepository.Update(productUnit);
            await _context.SaveChangesAsync();
        });

        var units = await _context.ProductUnits.ToListAsync();
        Assert.Empty(units);
    }

    [Fact]
    public async Task UpdateProductUnit_Null_Should_ThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
        {
            _productUnitRepository.Update(null);
            await _context.SaveChangesAsync();
        });
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
