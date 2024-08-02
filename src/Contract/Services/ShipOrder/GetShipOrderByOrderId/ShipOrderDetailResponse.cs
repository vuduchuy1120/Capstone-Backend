using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;
using Contract.Services.Set.SharedDto;

namespace Contract.Services.ShipOrder.GetShipOrderByOrderId;

public record ShipOrderDetailResponse(ProductResponse product, SetResponse set, int Quantity);

public record ShipOrderDetailWithImageLinkResponse(
    ProductWithOneImageResponse product,
    SetWithProductOneImageResponse set, 
    int Quantity);
