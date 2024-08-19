namespace Contract.Services.ProductPhase.SearchByThirdPartyCompany;

public record ProductWithTotalQuantityResponse(
    Guid Id,
    string Name,
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsInProcessing,
    string Image,
    int TotalQuantity, 
    int TotalAvailableQuantity);
