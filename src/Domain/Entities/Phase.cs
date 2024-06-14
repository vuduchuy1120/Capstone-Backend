using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Phase : EntityBase<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<ProductPhase> ProductPhases { get; set; }
    public List<EmployeeProduct> EmployeeProducts { get; set; }

}
