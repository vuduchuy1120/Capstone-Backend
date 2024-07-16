using Contract.Services.MaterialHistory.ShareDto;
using Contract.Services.Phase.ShareDto;
using Contract.Services.Product.SharedDto;

namespace Contract.Services.ShipmentDetail.Share;

public record DetailResponse(
    ProductResponse Product, 
    PhaseResponse Phase,
    MaterialHistoryResponse Material,
    double Quantity,
    ProductPhaseType ProductPhaseType,
    string ProductPhaseTypeDescription);
