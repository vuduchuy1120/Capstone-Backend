using Contract.Services.ShipOrder.Share;

namespace Contract.Services.ShipOrder.Create;

public record CreateShipOrderRequest(
    string ShipperId,
    DeliveryMethod KindOfShipOrder,
    Guid OrderId,
    DateTime ShipDate,
    List<ShipOrderDetailRequest> ShipOrderDetailRequests
    );
