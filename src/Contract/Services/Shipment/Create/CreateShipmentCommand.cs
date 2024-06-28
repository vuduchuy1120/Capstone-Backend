using Contract.Abstractions.Messages;

namespace Contract.Services.Shipment.Create;

public record CreateShipmentCommand(CreateShipmentRequest CreateShipmentRequest, string CreatedBy)
    : ICommand;