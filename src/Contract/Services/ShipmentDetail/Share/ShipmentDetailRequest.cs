namespace Contract.Services.ShipmentDetail.Share;

public record ShipmentDetailRequest(
    Guid ItemId,
    Guid? PhaseId, 
    int Quantity,
    KindOfShip KindOfShip);

