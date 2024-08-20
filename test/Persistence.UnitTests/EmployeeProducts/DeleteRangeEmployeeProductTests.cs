using Application.Abstractions.Data;
using Contract.Abstractions.Shared.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.EmployeeProducts;

public class DeleteRangeEmployeeProductTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IEmployeeProductRepository _employeeProductRepository;


    public DeleteRangeEmployeeProductTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _employeeProductRepository = new EmployeeProductRepository(_context);
    }

    [Fact]
    public async Task DeleteRangeEmployeeProduct_ShouldDeleteEmployeeProducts()
    {
        // Arrange
        var employeeProducts = new List<EmployeeProduct>
        {
            new EmployeeProduct
            {
                UserId = "1",
                SlotId = 1,
                Date = new DateOnly(2022, 1, 1),
                ProductId = Guid.NewGuid(),
                PhaseId = Guid.NewGuid(),
                Quantity = 10,
                CreatedBy = "huyvu",
                CreatedDate = DateUtils.GetNow()
            },
            new EmployeeProduct
            {
                UserId = "2",
                SlotId = 1,
                Date = new DateOnly(2022, 1, 1),
                ProductId = Guid.NewGuid(),
                PhaseId = Guid.NewGuid(),
                Quantity = 10,
                CreatedBy = "huyvu",
                CreatedDate = DateUtils.GetNow()
            }
        };

        await _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
        await _context.SaveChangesAsync();

        // Act
        _employeeProductRepository.DeleteRangeEmployeeProduct(employeeProducts);
        await _context.SaveChangesAsync();

        // Assert
        var employeeProductsInDb = await _context.EmployeeProducts.ToListAsync();
        Assert.Empty(employeeProductsInDb);
    }
    [Fact]
    public async Task DeleteRangeEmployeeProduct_ShouldDeleteEmployeeProducts_WhenEmployeeProductsAreEmpty()
    {
        // Arrange
        var employeeProducts = new List<EmployeeProduct>();

        // Act
        _employeeProductRepository.DeleteRangeEmployeeProduct(employeeProducts);
        await _context.SaveChangesAsync();

        // Assert
        var employeeProductsInDb = await _context.EmployeeProducts.ToListAsync();
        Assert.Empty(employeeProductsInDb);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
