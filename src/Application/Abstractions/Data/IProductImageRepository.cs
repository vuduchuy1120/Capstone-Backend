using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductImageRepository
{
    void Add(ProductImage productImage);
    void AddRange(List<ProductImage> productImages);
    void Delete(ProductImage productImage);
    void DeleteRange(List<ProductImage> productImages);
    Task<List<ProductImage>> GetByProductIdAsync(Guid productId);
    Task<List<ProductImage>> GetProductImageIdsAsync(List<Guid> productImageIds);
    Task<bool> IsAllImageIdExist(List<Guid> productImageIds, Guid productId);
}
