using Contract.Services.Shipment.Share;

namespace Contract.Services.ShipOrder.GetShipOrdersOfShipper;

public record SearchShipOrderOption(
    DateTime? ShipDate,
    Status Status,
    int PageIndex = 1,
    int PageSize = 10);
