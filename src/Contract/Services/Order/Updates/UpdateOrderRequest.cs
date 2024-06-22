namespace Contract.Services.Order.Updates;

public record UpdateOrderRequest
(
    Guid OrderId,
    Guid CompanyId,
    string Status,
    string StartOrder,
    string EndOrder);
