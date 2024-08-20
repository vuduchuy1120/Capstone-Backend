using Contract.Services.Phase.Creates;
using Contract.Services.Phase.Updates;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Phase : EntityBase<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<ProductPhase>? ProductPhases { get; set; }
    public List<EmployeeProduct>? EmployeeProducts { get; set; }
    public List<ShipmentDetail>? ShipmentDetails { get; set; }
    public List<ProductPhaseSalary>? ProductPhaseSalaries { get; set; }

    public static Phase Create(CreatePhaseRequest createPhaseRequest)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = createPhaseRequest.Name.Trim(),
            Description = createPhaseRequest.Description.Trim()
        };
    }
    public void Update(UpdatePhaseRequest updatePhaseRequest)
    {
        Name = updatePhaseRequest.Name.Trim();
        Description = updatePhaseRequest.Description.Trim();
    }
}
