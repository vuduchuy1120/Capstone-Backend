using Contract.Services.ShipmentDetail.Share;

namespace Contract.Services.Shipment.Create;

public record CreateShipmentRequest(
    Guid FromId, 
    Guid ToId,
    string ShipperId,
    DateTime ShipDate,
    List<ShipmentDetailRequest> ShipmentDetailRequests);