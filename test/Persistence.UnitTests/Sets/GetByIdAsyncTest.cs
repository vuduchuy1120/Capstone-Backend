using Application.Abstractions.Data;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.Set.CreateSet;
using Contract.Services.Set.SharedDto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

namespace Persistence.UnitTests.Sets;

public class GetByIdAsyncTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ISetRepository _setRepository;

    public GetByIdAsyncTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new AppDbContext(optionsBuilder.Options);
        _setRepository = new SetRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSetWithoutProducts_ShouldReturnSet()
    {
        // Arrange
        var createSetRequest = new CreateSetRequest("Code", "Name", "Description", "Image", null);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");
        var set = Set.Create(createSetCommand);
        _context.Sets.Add(set);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setRepository.GetByIdAsync(set.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(set.Id, result.Id);
        Assert.Equal(0, result.SetProducts?.Count());
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingSet_ShouldReturnNull()
    {
        // Act
        var result = await _setRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSetWithProducts_ShouldReturnSetWithProducts()
    {
        // Arrange
        var createProductRequest = new CreateProductRequest("Code", 123, 10, 10, "Size", "Description", "Name", null);
        var product1 = Product.Create(createProductRequest, "001201011091");
        var product2 = Product.Create(createProductRequest, "001201011091");

        _context.Products.Add(product1);
        _context.Products.Add(product2);

        var setProductsRequest = new List<SetProductRequest>()
        {
            new SetProductRequest(product1.Id, 5),
            new SetProductRequest(product2.Id, 5)
        };
        var createSetRequest = new CreateSetRequest("Code", "Name", "Description", "Image", setProductsRequest);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");
        var set = Set.Create(createSetCommand);

        _setRepository.Add(set);

        await _context.SaveChangesAsync();

        // Act
        var result = await _setRepository.GetByIdAsync(set.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(set.Id, result.Id);
        Assert.NotNull(result.SetProducts);
        Assert.Equal(2, result.SetProducts.Count);
        Assert.Contains(result.SetProducts, sp => sp.ProductId == product1.Id);
        Assert.Contains(result.SetProducts, sp => sp.ProductId == product2.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingSetWithProductsAndImages_ShouldReturnSetWithProductsAndImages()
    {
        // Arrange
        var imagesRequest = new List<ImageRequest>()
        {
            new ImageRequest("Image-1", true, false),
            new ImageRequest("Image-2", true, false)
        };
        var createProductRequest = new CreateProductRequest("Code", 123, 10, 10, "Size", "Description", "Name", imagesRequest);
        var product = Product.Create(createProductRequest, "001201011091");

        var setProductsRequest = new List<SetProductRequest>()
        {
            new SetProductRequest(product.Id, 5),
        };
        var createSetRequest = new CreateSetRequest("Code", "Name", "Description", "Image", setProductsRequest);
        var createSetCommand = new CreateSetCommand(createSetRequest, "CreatedBy");
        var set = Set.Create(createSetCommand);

        var productImage = imagesRequest.ConvertAll(i => ProductImage.Create(product.Id, i));

        _context.Sets.Add(set);
        _context.Products.Add(product);
        _context.ProductImages.AddRange(productImage);
        await _context.SaveChangesAsync();

        // Act
        var result = await _setRepository.GetByIdAsync(set.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(set.Id, result.Id);
        Assert.NotNull(result.SetProducts);
        var resultSetProduct = result.SetProducts.Single();
        Assert.NotNull(resultSetProduct.Product);
        Assert.NotNull(resultSetProduct.Product.Images);
        Assert.Equal(2, resultSetProduct.Product.Images.Count);
    }
}
