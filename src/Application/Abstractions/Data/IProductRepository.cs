using Contract.Services.Product.GetProducts;
using Contract.Services.Product.SearchWithSearchTerm;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductRepository
{
    void Add(Product product);
    void Update(Product product);
    Task<Product?> GetProductById(Guid id);
    Task<Product?> GetProductByIdWithProductPhase(Guid id);
    Task<bool> IsProductCodeExist(string code);
    Task<bool> IsAllSubProductIdsExist(List<Guid> SubProductIds);
    Task<bool> IsAllProductIdsExistAsync(List<Guid> productIds);
    Task<Product?> GetProductByIdWithoutImages(Guid id);
    Task<(List<Product>?, int)> SearchProductAsync(GetProductsQuery getProductsQuery);
    Task<bool> IsProductIdExist(Guid id);
    Task<(List<Product>, int)> SearchProductAsync(GetWithSearchTermQuery request);
    Task<bool> IsAllProductInProgress(List<Guid> productIds);
}
