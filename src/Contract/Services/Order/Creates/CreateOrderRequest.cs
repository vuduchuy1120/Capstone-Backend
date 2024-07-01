using Contract.Services.Order.ShareDtos;
using Contract.Services.OrderDetail.Creates;

namespace Contract.Services.Order.Creates;

public record CreateOrderRequest
(
    Guid CompanyId,
    StatusOrder Status,
    string StartOrder,
    string EndOrder,
    double VAT,
    CreateListOrderDetailsRequest CreateListOrderDetailsRequest
    );
