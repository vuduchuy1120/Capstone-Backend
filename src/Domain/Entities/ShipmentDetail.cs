using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipmentDetail : EntityBase<Guid>
{
    public Guid? ShipmentId { get; set; }
    public Guid? ShipOrderId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? SetId { get; set; }
    public Guid? MaterialHistoryId { get; set; }
    public Shipment? Shipment { get; set; }
    public ShipOrder? ShipOrder { get; set; }
    public Product? Product { get; set; }
    public Phase? Phase { get; set; }    
    public Set? Set { get; set; }
    public MaterialHistory? MaterialHistory { get; set; }
    public int Quantity { get; set; }
    public int ReturnQuantity { get; set; }
}
