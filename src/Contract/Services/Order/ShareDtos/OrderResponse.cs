using Contract.Services.Company.ShareDtos;

namespace Contract.Services.Order.ShareDtos;

public record OrderResponse
(
    Guid Id,
    Guid CompanyId,
    CompanyResponse Company,
    StatusOrder Status,
    string StatusType,
    string StatusDescription,
    DateOnly? StartOrder,
    DateOnly? EndOrder,
    double VAT);