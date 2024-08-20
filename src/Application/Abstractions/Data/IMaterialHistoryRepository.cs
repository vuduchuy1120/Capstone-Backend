using Contract.Services.Material.Get;
using Contract.Services.MaterialHistory.Queries;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IMaterialHistoryRepository
{
    // add material history repository methods
    void AddMaterialHistory(MaterialHistory materialHistory);
    void UpdateMaterialHistory(MaterialHistory materialHistory);
    void DeleteMaterialHistory(MaterialHistory materialHistory);
    Task<bool> IsMaterialHistoryExist(Guid id);
    Task<MaterialHistory?> GetMaterialHistoryByIdAsync(Guid id);
    Task<(List<MaterialHistory>?, int)> GetMaterialHistoriesByMaterialNameAndDateAsync(GetMaterialHistoriesByMaterialQuery getMaterialHistories);
}
