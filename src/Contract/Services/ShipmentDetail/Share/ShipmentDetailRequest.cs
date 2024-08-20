namespace Contract.Services.ShipmentDetail.Share;

public record ShipmentDetailRequest(
    Guid ItemId,
    Guid? PhaseId,
    double Quantity,
    decimal MaterialPrice,
    KindOfShip KindOfShip,
    ProductPhaseType ProductPhaseType);

