using Contract.Services.Company.ShareDtos;

namespace Contract.Services.Order.ShareDtos;

public record OrderResponse
(
    Guid Id,
    Guid CompanyId,
    CompanyResponse Company,
    string Status,
    DateOnly? StartOrder,
    DateOnly? EndOrder);