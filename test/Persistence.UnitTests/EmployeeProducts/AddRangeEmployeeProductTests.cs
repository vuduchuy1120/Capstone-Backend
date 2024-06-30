using Application.Abstractions.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.EmployeeProduct.Creates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Persistence.UnitTests.EmployeeProducts
{
    public class AddRangeEmployeeProductTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly IEmployeeProductRepository _employeeProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPhaseRepository _phaseRepository;
        private readonly ISlotRepository _slotRepository;
        private readonly IUserRepository _userRepository;

        public AddRangeEmployeeProductTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString());
            _context = new AppDbContext(optionsBuilder.Options);
            _employeeProductRepository = new EmployeeProductRepository(_context);
            _productRepository = new ProductRepository(_context);
            _phaseRepository = new PhaseRepository(_context);
            _slotRepository = new SlotRepository(_context);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task AddRangeEmployeeProduct_ShouldAddEmployeeProducts()
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

            // Act
            await _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
            await _context.SaveChangesAsync();

            // Assert
            var employeeProductsFromDb = await _context.EmployeeProducts.ToListAsync();
            Assert.Equal(2, employeeProductsFromDb.Count);
        }

        [Fact]
        public async Task AddRangeEmployeeProduct_WithNullElement_ShouldThrowArgumentException()
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
                    PhaseId = Guid.NewGuid()
                },
                null
            };

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                // Act
                await _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
                await _context.SaveChangesAsync();
            });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
