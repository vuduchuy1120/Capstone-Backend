using Contract.Services.Product.GetProducts;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductRepository
{
    void Add(Product product);
    void Update(Product product);
    Task<Product?> GetProductById (Guid id);
    Task<bool> IsProductCodeExist(string code);
    Task<bool> IsAllSubProductIdsExist(List<Guid> SubProductIds);
    Task<Product?> GetProductByIdWithoutImages(Guid id);
    Task<(List<Product>?, int)> SearchProductAsync(GetProductsQuery getProductsQuery);
    Task<bool> IsProductIdExist(Guid id);

}
