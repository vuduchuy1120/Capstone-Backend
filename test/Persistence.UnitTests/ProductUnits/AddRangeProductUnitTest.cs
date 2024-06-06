using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.ProductUnits;

public class AddRangeProductUnitTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductUnitRepository _productUnitRepository;

    public AddRangeProductUnitTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productUnitRepository = new ProductUnitRepository(_context);
    }
    [Fact]
    public void AddRange_Success_Should_AddProductUnitsToDb()
    {
        // Arrange
        var productId1 = Guid.NewGuid();
        var subProductId1 = Guid.NewGuid();
        var quantityPerUnit1 = 5;
        var productUnit1 = ProductUnit.Create(productId1, subProductId1, quantityPerUnit1);

        var productId2 = Guid.NewGuid();
        var subProductId2 = Guid.NewGuid();
        var quantityPerUnit2 = 10;
        var productUnit2 = ProductUnit.Create(productId2, subProductId2, quantityPerUnit2);

        var productUnits = new List<ProductUnit> { productUnit1, productUnit2 };

        // Act
        _productUnitRepository.AddRange(productUnits);
        _context.SaveChanges();

        // Assert
        var units = _context.ProductUnits.ToList();
        Assert.Equal(2, units.Count);
        Assert.Contains(units, u => u.ProductId == productId1 && u.SubProductId == subProductId1 && u.QuantityPerUnit == quantityPerUnit1);
        Assert.Contains(units, u => u.ProductId == productId2 && u.SubProductId == subProductId2 && u.QuantityPerUnit == quantityPerUnit2);
    }

    [Fact]
    public void AddRange_EmptyList_Should_DoNothing()
    {
        // Arrange
        var productUnits = new List<ProductUnit>();

        // Act
        _productUnitRepository.AddRange(productUnits);
        _context.SaveChanges();

        // Assert
        var units = _context.ProductUnits.ToList();
        Assert.Empty(units);
    }

    [Fact]
    public void AddRange_WithNullList_Should_ThrowArgumentNullException()
    {
        // Arrange
        List<ProductUnit> productUnits = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            _productUnitRepository.AddRange(productUnits);
            _context.SaveChanges();
        });
    }

    [Fact]
    public void AddRange_WithNullElement_Should_ThrowArgumentException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var subProductId = Guid.NewGuid();
        var quantityPerUnit = 5;
        var productUnit = ProductUnit.Create(productId, subProductId, quantityPerUnit);
        var productUnits = new List<ProductUnit> { productUnit, null };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            _productUnitRepository.AddRange(productUnits);
            _context.SaveChanges();
        });
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
