using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IProductPhaseSalaryRepository
{
    void Add(ProductPhaseSalary productPhaseSalary);
    void AddRange(List<ProductPhaseSalary> productPhaseSalaries);
    Task<ProductPhaseSalary> GetByProductIdAndPhaseId(Guid productId, Guid phaseId);
    void UpdateRange(List<ProductPhaseSalary> productPhaseSalaries);
    Task<List<ProductPhaseSalary>> GetAllProductPhaseSalaryAsync();

}
