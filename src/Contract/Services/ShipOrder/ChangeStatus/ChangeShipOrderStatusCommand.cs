using Contract.Abstractions.Messages;
using Contract.Services.Shipment.Share;

namespace Contract.Services.ShipOrder.ChangeStatus;

public record ChangeShipOrderStatusCommand(string UpdatedBy, Guid Id, ChangeShipOrderStatusRequest ChangeShipOrderStatusRequest) : ICommand;
