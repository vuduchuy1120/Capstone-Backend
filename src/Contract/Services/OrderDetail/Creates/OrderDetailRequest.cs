namespace Contract.Services.OrderDetail.Creates;

public record OrderDetailRequest
(
    Guid ProductIdOrSetId,
    int Quantity,
    decimal UnitPrice,
    string? Note,
    bool isProductId
    );
