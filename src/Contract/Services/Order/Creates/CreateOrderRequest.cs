using Contract.Services.Order.ShareDtos;

namespace Contract.Services.Order.Creates;

public record CreateOrderRequest
(
    Guid CompanyId,
    StatusOrder Status,
    string StartOrder,
    string EndOrder,
    double VAT
    );
