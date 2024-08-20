using Contract.Services.Product.SharedDto;

namespace Contract.Services.OrderDetail.ShareDtos;

public record SetOrderResponse
(
    Guid SetId,
    string SetCode,
    string SetName,
    string SetDescription,
    string ImageSetUrl,
    List<ProductResponse> ProductResponses,
    int Quantity,
    int ShippedQuantiy,
    decimal UnitPrice,
    string? Note
    );

