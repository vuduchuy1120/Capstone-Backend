
using Contract.Services.Material.Get;
using Contract.Services.Material.Share;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IMaterialRepository
{
    void AddMaterial(Material material);
    void UpdateMaterial(Material material);
    Task<Material?> GetMaterialByIdAsync(Guid id);
    Task<List<Material?>> GetMaterialsByNameAsync(string name);
    //check Material is exist
    Task<bool> IsMaterialExist(Guid id);
    //get distinct material Unit
    Task<List<string>> GetMaterialUnitsAsync();
    Task<(List<Material>?, int)> SearchMaterialsAsync(GetMaterialsQuery request);
    Task<bool> IsMaterialEnoughAsync(List<MaterialCheckQuantityRequest> requests);
    Task<List<Material>> GetMaterialsByIdsAsync(List<Guid> ids);
    void UpdateRange(List<Material> materials);
    Task<bool> IsMaterialNameExistedAsync(string name);
    Task<bool> IsUpdateMaterialNameExistedAsync(string name, Guid id);

}
