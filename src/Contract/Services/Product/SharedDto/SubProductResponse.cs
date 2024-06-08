namespace Contract.Services.Product.SharedDto;

public record SubProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    string Size,
    string Description,
    bool IsGroup,
    bool IsInProcessing,
    List<ImageResponse> ImageResponses);

