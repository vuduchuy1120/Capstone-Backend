using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductUnits;

public class AddProductUnitTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductUnitRepository _productUnitRepository;

    public AddProductUnitTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productUnitRepository = new ProductUnitRepository(_context);
    }
    [Fact]
    public async Task AddProductUnit_Success_Should_AddProductUnitToDb()
    {
        var productId = Guid.NewGuid();
        var subProductId = Guid.NewGuid();
        var quantityPerUnit = 5;
        var productUnit = ProductUnit.Create(productId, subProductId, quantityPerUnit);

        _productUnitRepository.Add(productUnit);
        await _context.SaveChangesAsync();

        var units = await _context.ProductUnits.ToListAsync();
        Assert.Single(units);
        Assert.Equal(productId, units[0].ProductId);
        Assert.Equal(subProductId, units[0].SubProductId);
        Assert.Equal(quantityPerUnit, units[0].QuantityPerUnit);
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}
