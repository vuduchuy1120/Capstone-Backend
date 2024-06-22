namespace Contract.Services.Order.Creates;

public record CreateOrderRequest
(
    Guid CompanyId,
    string Status,
    string StartOrder,
    string EndOrder
    );
