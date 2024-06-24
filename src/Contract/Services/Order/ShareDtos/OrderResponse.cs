namespace Contract.Services.Order.ShareDtos;

public record OrderResponse
(
    Guid Id,
    Guid CompanyId,
    string Status,
    DateOnly? StartOrder,
    DateOnly? EndOrder);