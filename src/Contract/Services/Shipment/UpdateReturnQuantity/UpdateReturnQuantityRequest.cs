using Contract.Services.ShipmentDetail.UpdateReturnQuantity;

namespace Contract.Services.Shipment.UpdateReturnQuantity;

public record UpdateReturnQuantityRequest(
    Guid ShipmentId,
    List<UpdateQuantityRequest> UpdateQuantityRequests);