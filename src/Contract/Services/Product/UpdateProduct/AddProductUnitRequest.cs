namespace Contract.Services.Product.UpdateProduct;

public record AddProductUnitRequest(Guid SubProductId, int QuantityPerUnit);