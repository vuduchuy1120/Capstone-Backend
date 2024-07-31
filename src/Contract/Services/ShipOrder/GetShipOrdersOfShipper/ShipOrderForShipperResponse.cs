using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Share;

namespace Contract.Services.ShipOrder.GetShipOrdersOfShipper;

public record ShipOrderForShipperResponse(
    Guid ShipOrderId,
    DateTime ShipDate,
    Status Status,
    string StatusDescription,
    DeliveryMethod DeliveryMethod,
    string DeliveryMethodDescription);
