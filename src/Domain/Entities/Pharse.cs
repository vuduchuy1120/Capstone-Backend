using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Pharse : EntityBase<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<ProductPharse> ProductPharses { get; set; }

}
