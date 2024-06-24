using Contract.Services.Shipment.Share;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Shipment : EntityAuditBase<Guid>
{
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
    public string ShipperId { get; set; }
    public User Shipper { get; set; }
    public DateTime ShipDate { get; set; }
    public Status Status { get; set; }
    public List<ShipmentDetail>? ShipmentDetails { get; set; }
}
