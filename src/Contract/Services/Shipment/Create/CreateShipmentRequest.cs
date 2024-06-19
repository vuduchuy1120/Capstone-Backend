using Contract.Services.ShipmentDetail.Share;

namespace Contract.Services.Shipment.Create;

public record CreateShipmentRequest(
    Guid FromId, 
    Guid ToId,
    List<ShipmentDetailRequest> ShipmentDetailRequests);