using Contract.Services.Shipment.Share;

namespace Contract.Services.Shipment.UpdateStatus;

public record UpdateStatusRequest(Guid ShipmentId, Status Status);
