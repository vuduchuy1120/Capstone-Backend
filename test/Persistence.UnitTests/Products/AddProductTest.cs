using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Products;

public class AddProductTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;
    public AddProductTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _productRepository = new ProductRepository(_context);
    }

    [Fact]
    public async Task AddProduct_Success_ShouldHaveNewProductInDb()
    {
        var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description", "Name", null);
        var product = Product.Create(createProductRequest, "001201011091");

        _productRepository.Add(product);
        _context.SaveChanges();

        Assert.Single(_context.Products);
        var savedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

        Assert.NotNull(savedProduct);
        Assert.Equal(product.Id, savedProduct.Id);
    }

    [Fact]
    public async Task AddProduct_IdExisted_Error_ShouldNotHaveNewUserToDb()
    {
        var createProductRequest = new CreateProductRequest("Code", 3434, "Size", "Description", "Name", null);
        var product = Product.Create(createProductRequest, "001201011091");

        _productRepository.Add(product);
        _context.SaveChanges();

        var duplicateProduct = Product.Create(createProductRequest, "001201011091");
        duplicateProduct.Id = product.Id;

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _productRepository.Add(duplicateProduct);
            await _context.SaveChangesAsync();
        });

        Assert.Single(_context.Products);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
