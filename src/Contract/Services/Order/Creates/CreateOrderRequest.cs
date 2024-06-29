using Contract.Services.Order.ShareDtos;

namespace Contract.Services.Order.Creates;

public record CreateOrderRequest
(
    Guid CompanyId,
    StatusType Status,
    string StartOrder,
    string EndOrder,
    double VAT
    );
