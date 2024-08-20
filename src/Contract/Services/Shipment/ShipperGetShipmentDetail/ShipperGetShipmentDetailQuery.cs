using Contract.Abstractions.Messages;
using Contract.Services.Shipment.GetShipmentDetail;

namespace Contract.Services.Shipment.ShipperGetShipmentDetail;

public record ShipperGetShipmentDetailQuery(string shipperId, Guid shipmentId) : IQuery<ShipmentDetailResponse>;
