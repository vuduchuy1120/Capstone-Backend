using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductRepository
{
    void Add(Product product);
    void Update(Product product);
    Task<Product?> GetProductById (Guid id);
    Task<bool> IsProductCodeExist(string code);
    Task<bool> IsHaveGroupInSubProductIds(List<Guid> SubProductIds);
    Task<bool> IsAllSubProductIdsExist(List<Guid> SubProductIds);
}
