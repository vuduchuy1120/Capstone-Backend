namespace Contract.Services.Product.CreateProduct;

public record CreateProductRequest(
    string Code, 
    decimal Price, 
    string Size, 
    string Description, 
    bool IsGroup,
    string Name,
    List<ProductUnitRequest> ProductUnitRequests,
    List<ImageRequest> ImageRequests);