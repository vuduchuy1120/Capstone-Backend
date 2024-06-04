namespace Contract.Services.Product.CreateProduct;

public record ProductUnitRequest(Guid SubProductId, int QuantityPerUnit);
