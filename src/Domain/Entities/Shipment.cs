using Contract.Services.Shipment.Create;
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

    public static Shipment Create(CreateShipmentRequest request, string createdBy)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            FromId = request.FromId,
            ToId = request.ToId,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            ShipperId = request.ShipperId,
            ShipDate = request.ShipDate,
        };
    }

    public void Update(string updateBy)
    {
        UpdatedBy = updateBy;
        UpdatedDate = DateTime.UtcNow;
    }
}
