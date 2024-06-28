using Contract.Services.ShipmentDetail.Share;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class ShipmentDetail : EntityBase<Guid>
{
    public Guid ShipmentId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? PhaseId { get; private set; }
    public Guid? MaterialHistoryId { get; private set; }
    public decimal MaterialPrice { get; private set; }
    public Shipment Shipment { get; private set; }
    public Product? Product { get; private set; }
    public Phase? Phase { get; private set; }
    public MaterialHistory? MaterialHistory { get; private set; }
    public double Quantity { get; private set; }
    public ProductPhaseType ProductPhaseType { get; private set; }
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
