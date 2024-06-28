using Domain.Entities;

namespace Application.Abstractions.Data;

public interface ISetProductRepository
{
    void AddRange(List<SetProduct> setProducts);
    void UpdateRange(List<SetProduct> setProducts);
    void DeleteRange(List<SetProduct> setProducts);
    Task<bool> DoProductIdsExistAsync(List<Guid> productIds, Guid setId);
    Task<bool> IsAnyIdExistAsync(List<Guid> productIds, Guid setId);
    Task<List<SetProduct>> GetByProductIdsAndSetId(List<Guid> productIds, Guid setId);
}
