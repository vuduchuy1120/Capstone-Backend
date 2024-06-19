using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipOrder : EntityAuditBase<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }    
    public List<ShipmentDetail>? ShipmentDetails { get; set; }
}
