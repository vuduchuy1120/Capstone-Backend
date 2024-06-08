using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Set : EntityAuditBase<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string ImageUrl { get; private set; }
    public string Description { get; private set; }
    public List<SetProduct>? SetProducts { get; private set; }
}
