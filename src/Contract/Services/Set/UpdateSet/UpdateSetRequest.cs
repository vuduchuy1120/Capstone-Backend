using Contract.Services.Set.SharedDto;

namespace Contract.Services.Set.UpdateSet;

public record UpdateSetRequest(
    Guid SetId, 
    string Code,
    string Name,
    string Description,
    string ImageUrl,
    List<SetProductRequest> Add,
    List<SetProductRequest> Update,
    List<Guid> RemoveProductIds);