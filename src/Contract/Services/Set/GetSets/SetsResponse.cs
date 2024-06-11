namespace Contract.Services.Set.GetSets;

public record SetsResponse(
    Guid Id,
    string Code,
    string Name,
    string ImageUrl,
    string Description);