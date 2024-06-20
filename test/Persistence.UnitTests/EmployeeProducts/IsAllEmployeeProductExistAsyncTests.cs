using Application.Abstractions.Data;
using Contract.Abstractions.Shared.Utils;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.EmployeeProducts;

public class IsAllEmployeeProductExistAsyncTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IEmployeeProductRepository _employeeProductRepository;

    public IsAllEmployeeProductExistAsyncTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _employeeProductRepository = new EmployeeProductRepository(_context);
    }

    [Fact]
    public async Task IsAllEmployeeProductExistAsync_ShouldReturnTrue_WhenAllEmployeeProductExist()
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
                CreatedDate = DateUtils.GetNow(),
                IsMold = false
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
                CreatedDate = DateUtils.GetNow(),
                IsMold = false
            }
        };

        await _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
        await _context.SaveChangesAsync();

        var keys = employeeProducts.Select(ep => new CompositeKey
        {
            UserId = ep.UserId,
            SlotId = ep.SlotId,
            Date = ep.Date.ToString("dd/MM/yyyy"),
            ProductId = ep.ProductId,
            PhaseId = ep.PhaseId
        }).ToList();

        // Act
        var result = await _employeeProductRepository.IsAllEmployeeProductExistAsync(keys);

        // Assert
        Assert.True(result);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
