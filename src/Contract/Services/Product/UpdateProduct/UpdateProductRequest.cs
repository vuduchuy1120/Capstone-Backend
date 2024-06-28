using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.UpdateProduct;

public record UpdateProductRequest(
    Guid Id, 
    string Code,
    decimal Price,
    string Size,
    string Description,
    string Name,
    bool IsInProcessing,
    List<ImageRequest>? AddImagesRequest,
    List<Guid>? RemoveImageIds);


