using Application.Abstractions.Data;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class EmployeeProductRepository : IEmployeeProductRepository
{
    private readonly AppDbContext _context;
    public EmployeeProductRepository(AppDbContext context)
    {
        _context = context;
    }
    public void AddEmployeeProduct(EmployeeProduct employeeProduct)
    {
        _context.EmployeeProducts.Add(employeeProduct);
    }

    public async Task AddRangeEmployeeProduct(List<EmployeeProduct> employeeProducts)
    {
        await _context.EmployeeProducts.AddRangeAsync(employeeProducts);
    }

    public void DeleteEmployeeProduct(EmployeeProduct employeeProduct)
    {
        _context.EmployeeProducts.Remove(employeeProduct);
    }

    public void DeleteRangeEmployeeProduct(IEnumerable<EmployeeProduct> employeeProducts)
    {
        _context.EmployeeProducts.RemoveRange(employeeProducts);
    }

    public void UpdateEmployeeProduct(EmployeeProduct employeeProduct)
    {
        _context.EmployeeProducts.Update(employeeProduct);
    }

    public void UpdateRangeEmployeeProduct(List<EmployeeProduct> employeeProducts)
    {
        _context.EmployeeProducts.UpdateRange(employeeProducts);
    }
}
