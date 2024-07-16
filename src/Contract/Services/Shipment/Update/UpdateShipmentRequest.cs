using Contract.Services.ShipmentDetail.Share;

namespace Contract.Services.Shipment.Update;

public record UpdateShipmentRequest(
    Guid ShipmentId,
    Guid FromId,
    Guid ToId,
    string ShipperId,
    DateTime ShipDate,
    List<ShipmentDetailRequest> ShipmentDetailRequests);
