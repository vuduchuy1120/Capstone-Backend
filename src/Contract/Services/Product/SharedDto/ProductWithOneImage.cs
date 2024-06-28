namespace Contract.Services.Product.SharedDto;

public record ProductWithOneImage(Guid Id,
    string Name,
    string Code,
    decimal Price,
    string Size,
    string Description,
    bool IsInProcessing,
    string ImageUrl);
