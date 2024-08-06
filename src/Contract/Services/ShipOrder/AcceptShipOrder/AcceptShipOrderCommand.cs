using Contract.Abstractions.Messages;

namespace Contract.Services.ShipOrder.AcceptShipOrder;

public record AcceptShipOrderCommand(Guid shipOrderId, string updatedBy) : ICommand;
