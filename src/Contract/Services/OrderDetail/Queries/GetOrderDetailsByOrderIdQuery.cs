using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.OrderDetail.ShareDtos;

namespace Contract.Services.OrderDetail.Queries;

public record GetOrderDetailsByOrderIdQuery
(
    Guid OrderId
    ) : IQuery<OrderDetailResponse>;
