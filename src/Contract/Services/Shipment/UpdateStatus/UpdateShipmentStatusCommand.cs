using Contract.Abstractions.Messages;

namespace Contract.Services.Shipment.UpdateStatus;

public record UpdateShipmentStatusCommand(Guid Id, UpdateStatusRequest UpdateStatusRequest, string UpdatedBy) : ICommand;
