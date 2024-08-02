using Contract.Abstractions.Messages;

namespace Contract.Services.ShipOrder.GetShipOrderDetail;

public record GetShipOrderDetailQuery(Guid ShipOrderId) : IQuery<DetailShipOrderResponse>;
