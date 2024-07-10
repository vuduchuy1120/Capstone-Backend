using Contract.Services.Product.SharedDto;

namespace Contract.Services.Set.SharedDto;

public record SetProductResponse(
    Guid SetId,
    Guid ProductId,
    int Quantity,
    ProductResponse Product);


public record SetProductWithProductSalaryResponse(
    Guid SetId,
    Guid ProductId,
    int Quantity,
    ProductWithQuantityResponse Product);
