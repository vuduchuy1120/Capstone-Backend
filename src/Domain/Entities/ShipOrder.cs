using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Create;
using Contract.Services.ShipOrder.Share;
using Contract.Services.ShipOrder.Update;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipOrder : EntityAuditBase<Guid>
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }    
    public string ShipperId { get; private set; }
    public User Shipper { get; private set; }
    public DateTime ShipDate { get; private set; }
    public Status Status { get; private set; }
    public bool IsAccepted { get; private set; } = false;
    public DeliveryMethod DeliveryMethod { get; private set; }
    public List<ShipOrderDetail>? ShipOrderDetails { get; private set; }

    private ShipOrder()
    {
    }

    public static ShipOrder Create(string createdBy, CreateShipOrderRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            OrderId = request.OrderId,
            ShipperId = request.ShipperId,
            ShipDate = request.ShipDate,
            Status = Status.WAIT_FOR_SHIP,
            DeliveryMethod = request.KindOfShipOrder,
            IsAccepted = false
        };
    }

    public void Update(string updatedBy, List<ShipOrderDetail>? shipOrderDetails, UpdateShipOrderRequest request)
    {
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
        ShipperId = request.ShipperId;
        ShipDate = request.ShipDate;
        DeliveryMethod = request.KindOfShipOrder;
        OrderId = request.OrderId;
        ShipOrderDetails = shipOrderDetails;
    }

    public void UpdateStatus(Status status, string updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
        Status = status;
    }

    public void UpdateAccepted(string updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedDate = DateTime.UtcNow;
        IsAccepted = true;
    }
}
