using Contract.Services.OrderDetail.Creates;

namespace Contract.Services.OrderDetail.Updates;

public record UpdateOrderDetailRequest
(   Guid OrderDetailId,
    List<OrderDetailRequest> OrderDetailRequests
    );
