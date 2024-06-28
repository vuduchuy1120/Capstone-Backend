using Contract.Abstractions.Messages;
using Contract.Services.ShipmentDetail.Share;

namespace Contract.Services.Shipment.Update;

public record UpdateShipmentCommand(
    Guid Id,
    UpdateShipmentRequest UpdateShipmentRequest, 
    string UpdatedBy) : ICommand;
