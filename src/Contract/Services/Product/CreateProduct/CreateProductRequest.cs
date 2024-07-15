using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.CreateProduct;

public record CreateProductRequest(
    string Code, 
    decimal PriceFinished,
    decimal PricePhase1,
    decimal PricePhase2,
    string Size, 
    string Description, 
    string Name,
    List<ImageRequest>? ImageRequests);