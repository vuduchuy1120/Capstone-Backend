using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Data;

public interface IEmployeeProductRepository
{
    void AddEmployeeProduct(EmployeeProduct employeeProduct);
    // AddRange
    Task AddRangeEmployeeProduct(List<EmployeeProduct> employeeProducts);
    // Update
    void UpdateEmployeeProduct(EmployeeProduct employeeProduct);
    // Delete
    void DeleteEmployeeProduct(EmployeeProduct employeeProduct);
    // DeleteRange
    void DeleteRangeEmployeeProduct(List<EmployeeProduct> employeeProducts);
    // IsAllEmployeeProductExistAsync
    Task<bool> IsAllEmployeeProductExistAsync(List<CompositeKey> keys);
    Task<List<EmployeeProduct>> GetEmployeeProductsByKeysAsync(List<CompositeKey> keys);
    void UpdateRangeEmployeeProduct(List<EmployeeProduct> employeeProducts);
    Task<List<EmployeeProduct>> GetEmployeeProductsByEmployeeIdDateAndSlotId(string userId, int slotId, DateOnly date);
    Task<List<EmployeeProduct>> GetEmployeeProductsByDateAndSlotId(int slotId, DateOnly date);
}
