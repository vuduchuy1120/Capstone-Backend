namespace Contract.Services.Product.UpdateProduct;

public record ActionAddRequest(List<AddProductImageRequest>? Images, List<AddProductUnitRequest>? ProductUnits);