using Contract.Services.Shipment.Share;

namespace Contract.Services.ShipOrder.ChangeStatus;

public record ChangeShipOrderStatusRequest(Guid ShipOrderId, Status Status);