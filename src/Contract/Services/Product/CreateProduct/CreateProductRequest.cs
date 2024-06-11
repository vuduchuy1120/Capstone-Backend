using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.CreateProduct;

public record CreateProductRequest(
    string Code, 
    decimal Price, 
    string Size, 
    string Description, 
    string Name,
    List<ImageRequest>? ImageRequests);