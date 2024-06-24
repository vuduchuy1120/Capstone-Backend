using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipmentDetail : EntityBase<Guid>
{
    public Guid? ShipmentId { get; private set; }
    public Guid? ShipOrderId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? PhaseId { get; private set; }
    public Guid? SetId { get; private set; }
    public Guid? MaterialHistoryId { get; set; }
    public Shipment? Shipment { get; private set; }
    public ShipOrder? ShipOrder { get; private set; }
    public Product? Product { get; private set; }
    public Phase? Phase { get; private set; }
    public Set? Set { get; private set; }
    public MaterialHistory? MaterialHistory { get; private set; }
    public int Quantity { get; private set; }
    public ProductPhaseType ProductPhaseType { get; set; }
    private ShipmentDetail() { }

    public static ShipmentDetail CreateShipmentProductDetail(
        Guid shipmentId, ShipmentDetailRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipmentId,
            ProductId = request.ItemId,
            PhaseId = request.PhaseId,
            Quantity = request.Quantity,
            ProductPhaseType = request.ProductPhaseType,
        };
    }

    public static ShipmentDetail CreateShipOrderProductDetail(
        Guid shipId, ShipmentDetailRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            ShipOrderId = shipId,
            ProductId = request.ItemId,
            PhaseId = request.PhaseId,
            Quantity = request.Quantity,
            ProductPhaseType = request.ProductPhaseType,
        };
    }

    public static ShipmentDetail CreateShipmentSetDetail(
        Guid shipId, ShipmentDetailRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipId,
            SetId = request.ItemId,
            Quantity = request.Quantity,
        };
    }

    public static ShipmentDetail CreateShipOrderSetDetail(
        Guid shipId, Guid itemId, int quantity)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            ShipOrderId = shipId,
            SetId = itemId,
            Quantity = quantity,
        };
    }

    public static ShipmentDetail CreateShipmentMaterialDetail(
        Guid shipId, ShipmentDetailRequest request)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            ShipmentId = shipId,
            MaterialHistoryId = request.ItemId,
            Quantity = request.Quantity,
        };
    }

}
