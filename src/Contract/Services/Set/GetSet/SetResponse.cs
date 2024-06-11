using Contract.Services.Set.SharedDto;

namespace Contract.Services.Set.GetSet;

public record SetResponse(
    Guid Id,
    string Code,
    string Name,
    string ImageUrl,
    string Description,
    List<SetProductResponse> SetProducts);
