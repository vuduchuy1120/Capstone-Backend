
using Contract.Abstractions.Messages;

namespace Contract.Services.ShipOrder.Update;

public record UpdateShipOderCommand(string UpdatedBy, Guid ShipOrderId, UpdateShipOrderRequest UpdateShipOrderRequest)
    : ICommand;