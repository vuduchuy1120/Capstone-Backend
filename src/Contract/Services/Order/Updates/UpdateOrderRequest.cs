using Contract.Services.Order.ShareDtos;

namespace Contract.Services.Order.Updates;

public record UpdateOrderRequest
(
    Guid OrderId,
    Guid CompanyId,
    StatusOrder Status,
    double VAT,
    string StartOrder,
    string EndOrder);
