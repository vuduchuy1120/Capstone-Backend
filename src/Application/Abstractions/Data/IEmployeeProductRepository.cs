using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IEmployeeProductRepository
{
    Task AddRangeEmployeeProduct(List<EmployeeProduct> employeeProducts);
    void DeleteRangeEmployeeProduct(List<EmployeeProduct> employeeProducts);
    Task<bool> IsAllEmployeeProductExistAsync(List<CompositeKey> keys);
    Task<List<EmployeeProduct>> GetEmployeeProductsByKeysAsync(List<CompositeKey> keys);
    Task<List<EmployeeProduct>> GetEmployeeProductsByEmployeeIdDateAndSlotId
        (string userId, int slotId, DateOnly date, Guid companyId);
    Task<List<EmployeeProduct>> GetEmployeeProductsByDateAndSlotId(int slotId, DateOnly date, Guid companyId);
}
