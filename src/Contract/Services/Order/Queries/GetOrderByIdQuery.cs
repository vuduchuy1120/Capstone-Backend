using Contract.Abstractions.Messages;
using Contract.Services.Order.ShareDtos;

namespace Contract.Services.Order.Queries;

public record GetOrderByIdQuery(Guid Id) : IQueryHandler<OrderResponse>;
