namespace Contract.Services.Product.SharedDto;

public record ProductResponse(
    SubProductResponse Product,
    List<SubProductResponse> SubProducts);