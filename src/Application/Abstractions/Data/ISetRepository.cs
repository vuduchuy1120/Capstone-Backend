using Contract.Services.Set.GetSets;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface ISetRepository
{
    void Add(Set set);
    void Update(Set set);
    Task<bool> IsCodeExistAsync(string code);
    Task<Set?> GetByIdAsync(Guid id);
    Task<Set?> GetByIdWithoutSetProductAsync(Guid id);
    Task<(List<Set>, int)> SearchSetAsync(GetSetsQuery request);
    Task<List<Set>> SearchSetAsync(string searchTerm);
    Task<bool> IsAllSetIdExistAsync(List<Guid> setIds);
    Task<bool> IsAllSetExistAsync(List<Guid> ids);
}
