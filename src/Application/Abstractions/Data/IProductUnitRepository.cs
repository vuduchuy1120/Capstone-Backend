using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductUnitRepository
{
    void Add(ProductUnit productUnit);
    void Update(ProductUnit productUnit);
    void Delete(ProductUnit productUnit);
    void AddRange(List<ProductUnit> productUnits);
    void DeleteRange(List<ProductUnit> productUnits);   
    Task<List<ProductUnit>> GetBySubProductIdsAsync(Guid productId, List<Guid> subProductIds);
}
