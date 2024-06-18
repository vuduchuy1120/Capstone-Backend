using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Shipment : EntityAuditBase<Guid>
{
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
    public List<ShipmentDetail>? ShipmentDetails { get; set; }
}
