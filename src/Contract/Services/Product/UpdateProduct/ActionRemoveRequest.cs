namespace Contract.Services.Product.UpdateProduct;

public record ActionRemoveRequest(List<Guid> ImageIds, List<Guid> SubProductIds);