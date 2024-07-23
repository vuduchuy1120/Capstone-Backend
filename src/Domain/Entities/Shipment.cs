using Contract.Services.Shipment.Create;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.Update;
using Domain.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Shipment : EntityAuditBase<Guid>
{
    public Guid FromId { get; private set; }
    public Guid ToId { get; private set; }
    public string ShipperId { get; set; }
    public User Shipper { get; set; }
    public DateTime ShipDate { get; set; }
    public Status Status { get; set; }
    public bool IsAccepted { get; set; } = false;
    public Company FromCompany { get; set; }
    public Company ToCompany { get; set; }
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
            IsAccepted = false
        };
    }

    public void UpdateStatus(string updateBy, Status status)
    {
        Status = status;
        UpdatedBy = updateBy;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Update(UpdateShipmentRequest request, string updatedBy, List<ShipmentDetail> newShipmentDetails)
    {
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
        FromId = request.FromId;
        ToId = request.ToId;
        ShipperId = request.ShipperId;
        ShipDate = request.ShipDate;
        ShipmentDetails = newShipmentDetails;
    }

    public void UpdateAccepted(string updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
        IsAccepted = true;
    }
}
