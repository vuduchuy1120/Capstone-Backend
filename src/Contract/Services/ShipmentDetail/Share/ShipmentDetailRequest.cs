namespace Contract.Services.ShipmentDetail.Share;

public record ShipmentDetailRequest(
    Guid ItemId,
    Guid? PhaseId,
    double Quantity,
    KindOfShip KindOfShip,
    ProductPhaseType ProductPhaseType);

