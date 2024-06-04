namespace Contract.Services.Product.UpdateProduct;

public record UpdateProductRequest(
    Guid Id, 
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsGroup,
    string Name);