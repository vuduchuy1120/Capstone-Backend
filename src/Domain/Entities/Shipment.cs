using Contract.Services.Shipment.Share;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Shipment : EntityAuditBase<Guid>
{
    public Guid FromId { get; private set; }
    public Guid ToId { get; private set; }
    public string ShipperId { get; set; }
    public User Shipper { get; set; }
    public DateTime ShipDate { get; set; }
    public Status Status { get; set; }
    public List<ShipmentDetail>? ShipmentDetails { get; set; }
    private Shipment()
    {
    }

    public static Shipment Create(Guid fromId, Guid toId, string createdBy)
    {
        return new()
        {
            FromId = fromId,
            ToId = toId,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
        };
    }

    public void Update(string updateBy)
    {
        UpdatedBy = updateBy;
        UpdatedDate = DateTime.UtcNow;
    }
}
