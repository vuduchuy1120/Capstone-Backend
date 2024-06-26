using Contract.Services.Product.SharedDto;

namespace Contract.Services.OrderDetail.ShareDtos;

public record SetOrderResponse
(
    Guid SetId,
    string SetName,
    string SetDescription,
    string ImageSetUrl,
    List<ProductResponse> ProductResponses,
    int Quantity,
    decimal UnitPrice,
    string? Note
    );

