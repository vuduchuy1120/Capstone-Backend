namespace Contract.Services.ShipmentDetail.UpdateReturnQuantity;

public record UpdateQuantityRequest(Guid ShipDetailId, int Quantity);