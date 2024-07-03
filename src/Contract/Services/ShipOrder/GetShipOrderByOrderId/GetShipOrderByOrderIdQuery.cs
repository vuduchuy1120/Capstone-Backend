using Contract.Abstractions.Messages;

namespace Contract.Services.ShipOrder.GetShipOrderByOrderId;

public record GetShipOrderByOrderIdQuery(Guid id) : IQuery<List<ShipOrderResponse>>;