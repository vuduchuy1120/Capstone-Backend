using Contract.Services.Shipment.Share;
using Contract.Services.ShipmentDetail.Share;

namespace Contract.Services.Shipment.Update;

public record UpdateShipmentRequest(
    Guid ShipmentId,
    Guid FromId,
    Guid ToId,
    string ShipperId,
    DateTime ShipDate,
    Status Status,
    List<ShipmentDetailRequest> ShipmentDetailRequests);
