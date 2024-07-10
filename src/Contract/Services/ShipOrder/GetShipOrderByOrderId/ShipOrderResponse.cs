using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.Share;

namespace Contract.Services.ShipOrder.GetShipOrderByOrderId;

public record ShipOrderResponse(
    Guid ShipOrderId,
    string ShipperId, 
    string ShipperName,
    DateTime ShipDate,
    Status Status,
    string StatusDescription,
    DeliveryMethod DeliveryMethod,
    string DeliveryMethodDescription,
    List<ShipOrderDetailResponse> ShipOrderDetailResponses);