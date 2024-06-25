using Contract.Abstractions.Messages;

namespace Contract.Services.Shipment.GetShipmentDetail;

public record GetShipmentDetailQuery(Guid ShipmentId) : IQuery<ShipmentDetailResponse>;