using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;

namespace Contract.Services.ShipOrder.GetShipOrderByOrderId;

public record ShipOrderDetailResponse(ProductResponse product, SetResponse set, int Quantity);
