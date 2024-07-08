using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Share;

namespace Contract.Services.ShipOrder.Update;

public record UpdateShipOrderRequest(
    Guid Id,
    string ShipperId,
    DeliveryMethod KindOfShipOrder,
    Guid OrderId,
    DateTime ShipDate,
    List<ShipOrderDetailRequest> ShipOrderDetailRequests);