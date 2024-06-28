
using Contract.Services.Material.Get;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IMaterialRepository
{
    void AddMaterial(Material material);
    void UpdateMaterial(Material material);
    Task<Material?> GetMaterialByIdAsync(int id);
    Task<List<Material?>> GetMaterialsByNameAsync(string name);
    //check Material is exist
    Task<bool> IsMaterialExist(int id);
    //get distinct material Unit
    Task<List<string>> GetMaterialUnitsAsync();
    Task<(List<Material>?, int)> SearchMaterialsAsync(GetMaterialsQuery request);
}
