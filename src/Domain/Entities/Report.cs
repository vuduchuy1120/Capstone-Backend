using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Report : EntityAuditBase<Guid>
{
    public string Description { get; set; }
    public string Status { get; set; }
}
