using Contract.Abstractions.Messages;
using Contract.Services.Shipment.UpdateStatus;

namespace Contract.Services.Shipment.ShipperUpdateStatus;

public record ShipperUpdateStatusCommand(string shipperId, Guid Id, UpdateStatusRequest updateStatus) : ICommand;
