//using Application.Abstractions.Data;
//using Contract.Abstractions.Shared.Utils;
//using Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Persistence.Repositories;

//namespace Persistence.UnitTests.EmployeeProducts
//{
//    public class EmployeeProductRepositoryTests : IDisposable
//    {
//        private readonly AppDbContext _context;
//        private readonly IEmployeeProductRepository _employeeProductRepository;

//        public EmployeeProductRepositoryTests()
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
//                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                        .EnableSensitiveDataLogging();
//            _context = new AppDbContext(optionsBuilder.Options);
//            _employeeProductRepository = new EmployeeProductRepository(_context);
//        }

//        [Fact]
//        public async Task GetEmployeeProductsByKeysAsync_ShouldReturnCorrectEmployeeProducts()
//        {
//            // Arrange
//            var employeeProducts = new List<EmployeeProduct>
//            {
//                new EmployeeProduct
//                {
//                    UserId = "1",
//                    SlotId = 1,
//                    Date = new DateOnly(2022, 1, 1),
//                    ProductId = Guid.NewGuid(),
//                    PhaseId = Guid.NewGuid(),
//                    Quantity = 10,
//                    CreatedBy = "huyvu",
//                    CreatedDate = DateUtils.GetNow(),
//                    IsMold = false
//                },
//                new EmployeeProduct
//                {
//                    UserId = "2",
//                    SlotId = 1,
//                    Date = new DateOnly(2022, 1, 1),
//                    ProductId = Guid.NewGuid(),
//                    PhaseId = Guid.NewGuid(),
//                    Quantity = 10,
//                    CreatedBy = "huyvu",
//                    CreatedDate = DateUtils.GetNow(),
//                    IsMold = false
//                }
//            };

//            await _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
//            await _context.SaveChangesAsync();

//            var keys = employeeProducts.Select(ep => new CompositeKey
//            {
//                UserId = ep.UserId,
//                SlotId = ep.SlotId,
//                Date = ep.Date.ToString("dd/MM/yyyy"),
//                ProductId = ep.ProductId,
//                PhaseId = ep.PhaseId
//            }).ToList();

//            // Act
//            var result = await _employeeProductRepository.GetEmployeeProductsByKeysAsync(keys);

//            // Assert
//            Assert.Equal(2, result.Count);
//        }

//        [Fact]
//        public async Task GetEmployeeProductsByEmployeeIdDateAndSlotId_ShouldReturnCorrectEmployeeProducts()
//        {
//            // Arrange
//            var employeeProduct = new EmployeeProduct
//            {
//                UserId = "1",
//                SlotId = 1,
//                Date = new DateOnly(2022, 1, 1),
//                ProductId = Guid.NewGuid(),
//                PhaseId = Guid.NewGuid(),
//                Quantity = 10,
//                CreatedBy = "huyvu",
//                CreatedDate = DateUtils.GetNow(),
//                IsMold = false
//            };

//            await _employeeProductRepository.AddRangeEmployeeProduct(new List<EmployeeProduct> { employeeProduct });
//            await _context.SaveChangesAsync();

//            // Act
//            var result = await _employeeProductRepository.GetEmployeeProductsByEmployeeIdDateAndSlotId(employeeProduct.UserId, employeeProduct.SlotId, employeeProduct.Date);

//            // Assert
//            Assert.NotNull(result);
//        }
//    //    [Fact]
//    //    public async Task GetEmployeeProductsByDateAndSlotId_ShouldReturnCorrectEmployeeProducts()
//    //    {
//    //        // Arrange
//    //        var employeeProducts = new List<EmployeeProduct>
//    //{
//    //    new EmployeeProduct
//    //    {
//    //        UserId = "1",
//    //        SlotId = 1,
//    //        Date = new DateOnly(2022, 1, 1),
//    //        ProductId = Guid.NewGuid(),
//    //        PhaseId = Guid.NewGuid(),
//    //        Quantity = 10,
//    //        CreatedBy = "huyvu",
//    //        CreatedDate = DateUtils.GetNow(),
//    //        IsMold = false
//    //    },
//    //    new EmployeeProduct
//    //    {
//    //        UserId = "2",
//    //        SlotId = 1,
//    //        Date = new DateOnly(2022, 1, 1),
//    //        ProductId = Guid.NewGuid(),
//    //        PhaseId = Guid.NewGuid(),
//    //        Quantity = 10,
//    //        CreatedBy = "huyvu",
//    //        CreatedDate = DateUtils.GetNow(),
//    //        IsMold = false
//    //    }
//    //};

//    //        await _employeeProductRepository.AddRangeEmployeeProduct(employeeProducts);
//    //        await _context.SaveChangesAsync();

//    //        // Act
//    //        var result = await _employeeProductRepository.GetEmployeeProductsByDateAndSlotId(1, new DateOnly(2022, 1, 1));

//    //        // Assert
//    //        Assert.Equal(2, result.Count);
//    //    }

//        public void Dispose()
//        {
//            _context.Dispose();
//        }
//    }
//}
