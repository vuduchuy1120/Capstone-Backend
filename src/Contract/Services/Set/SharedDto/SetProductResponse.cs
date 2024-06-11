using Contract.Services.Product.SharedDto;

namespace Contract.Services.Set.SharedDto;

public record SetProductResponse(
    Guid SetId,
    Guid ProductId,
    int Quantity,
    ProductResponse Product);