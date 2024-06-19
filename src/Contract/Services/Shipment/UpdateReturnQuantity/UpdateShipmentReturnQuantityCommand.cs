using Contract.Abstractions.Messages;
using Contract.Services.ShipmentDetail.UpdateReturnQuantity;

namespace Contract.Services.Shipment.UpdateReturnQuantity;

public record UpdateShipmentReturnQuantityCommand(
    string UpdatedBy,
    Guid ShipmentId,
    UpdateReturnQuantityRequest UpdateReturnQuantityRequest) : ICommand;