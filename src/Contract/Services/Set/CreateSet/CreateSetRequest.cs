using Contract.Services.Set.SharedDto;

namespace Contract.Services.Set.CreateSet;

public record CreateSetRequest(
    string Code, 
    string Name, 
    string Description,
    string ImageUrl,
    List<SetProductRequest> SetProductsRequest);
