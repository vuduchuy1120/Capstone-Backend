namespace Contract.Services.OrderDetail.ShareDtos;

public record OrderDetailResponse
(
    Guid OrderId,
    List<ProductOrderResponse> ProductOrderResponses,
    List<SetOrderResponse> SetOrderResponses,
    string? Note
    );


