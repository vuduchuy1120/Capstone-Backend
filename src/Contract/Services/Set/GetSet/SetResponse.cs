using Contract.Services.Product.SharedDto;
using Contract.Services.Set.SharedDto;

namespace Contract.Services.Set.GetSet;

public record SetResponse(
    Guid Id,
    string Code,
    string Name,
    string ImageUrl,
    string Description,
    List<SetProductResponse> SetProducts);

public record SetWithProductOneImageResponse(
    Guid Id,
    string Code,
    string Name,
    string ImageUrl,
    string Description,
    List<ProductWithOneImageResponse> SetProducts);

public record SetsWithProductSalaryResponse(
    Guid Id,
    string Code,
    string Name,
    string ImageUrl,
    string Description,
    List<SetProductWithProductSalaryResponse> SetProducts);
