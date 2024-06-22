namespace Contract.Services.Order.ShareDtos;

public record OrderResponse
(
    Guid OrderId,
    Guid CompanyId,
    string Status,
    string StartOrder,
    string EndOrder);