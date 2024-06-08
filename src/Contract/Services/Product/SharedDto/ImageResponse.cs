namespace Contract.Services.Product.SharedDto;

public record ImageResponse(Guid Id, string ImageUrl, bool IsBluePrint, bool IsMainImage);