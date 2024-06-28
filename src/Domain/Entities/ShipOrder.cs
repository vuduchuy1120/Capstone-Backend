using Contract.Services.Shipment.Share;
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
    public List<ShipmentDetail>? ShipmentDetails { get; set; }
}
