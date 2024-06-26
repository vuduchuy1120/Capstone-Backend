namespace Contract.Services.Order.Updates;

public record UpdateOrderRequest
(
    Guid OrderId,
    Guid CompanyId,
    string Status,
    double VAT,
    string StartOrder,
    string EndOrder);
