using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductImageRepository
{
    void Add(ProductImage productImage);
    void AddRange(List<ProductImage> productImages);
    void Update(ProductImage productImage);
    void Delete(ProductImage productImage);
    Task<ProductImage> GetByIdAsync(Guid id);
    Task<List<ProductImage>> GetByProductIdAsync(Guid productId);
}
