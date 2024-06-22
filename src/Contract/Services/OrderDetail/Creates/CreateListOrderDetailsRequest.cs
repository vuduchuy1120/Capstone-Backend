namespace Contract.Services.OrderDetail.Creates;

public record CreateListOrderDetailsRequest
(
    Guid OrderId,
    List<OrderDetailRequest> OrderDetailRequests
    );
