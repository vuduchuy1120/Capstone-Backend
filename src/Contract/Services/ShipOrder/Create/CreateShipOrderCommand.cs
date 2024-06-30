using Contract.Abstractions.Messages;

namespace Contract.Services.ShipOrder.Create;

public record CreateShipOrderCommand(string createdBy, CreateShipOrderRequest CreateShipOrderRequest) : ICommand;
