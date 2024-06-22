namespace Contract.Services.OrderDetail.Creates;

public record OrderDetailRequest
(
    Guid? ProductId,
    Guid? SetId,
    int Quantity
    );
