namespace Contract.Services.ShipOrder.Share;

public record ShipOrderDetailRequest(Guid ItemId, int Quantity, ItemKind ItemKind);
