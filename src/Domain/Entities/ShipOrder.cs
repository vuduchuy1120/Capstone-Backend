using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Share;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipOrder : EntityAuditBase<Guid>
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }    
    public string ShipperId { get; set; }
    public User Shipper { get; set; }
    public DateTime ShipDate { get; set; }
    public Status Status { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public List<ShipOrderDetail>? ShipOrderDetails { get; set; }
}
