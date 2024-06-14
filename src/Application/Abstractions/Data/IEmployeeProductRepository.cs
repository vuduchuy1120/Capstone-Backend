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
    // UpdateRange
    void UpdateRangeEmployeeProduct(List<EmployeeProduct> employeeProducts);
    // Delete
    void DeleteEmployeeProduct(EmployeeProduct employeeProduct);
    // DeleteRange
    void DeleteRangeEmployeeProduct(IEnumerable<EmployeeProduct> employeeProducts);
}
