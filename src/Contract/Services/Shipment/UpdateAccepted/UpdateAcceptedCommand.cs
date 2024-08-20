using Contract.Abstractions.Messages;

namespace Contract.Services.Shipment.UpdateAccepted;

public record UpdateAcceptedCommand(string updatedBy, Guid shipmentId) : ICommand;
