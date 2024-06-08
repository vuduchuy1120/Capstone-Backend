using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.UpdateProduct;

public record ActionAddRequest(List<ImageRequest>? Images, List<ProductUnitRequest>? ProductUnits);