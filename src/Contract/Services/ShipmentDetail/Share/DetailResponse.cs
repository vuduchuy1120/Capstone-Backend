using Contract.Services.MaterialHistory.ShareDto;
using Contract.Services.Phase.ShareDto;
using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;

namespace Contract.Services.ShipmentDetail.Share;

public record DetailResponse(
    ProductResponse Product, 
    PhaseResponse Phase,
    SetResponse Set, 
    MaterialHistoryResponse Material,
    int Quantity);
