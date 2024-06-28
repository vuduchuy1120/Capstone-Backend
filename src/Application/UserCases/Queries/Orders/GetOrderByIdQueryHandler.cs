using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Order.Queries;
using Contract.Services.Order.ShareDtos;
using Domain.Exceptions.Orders;

namespace Application.UserCases.Queries.Orders;

public record GetOrderByIdQueryHandler
    (IOrderRepository _orderRepository, IMapper _mapper)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<Result.Success<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.Id);
        if (order == null)
        {
            throw new OrderNotFoundException(request.Id);
        }
        var orderResponse = _mapper.Map<OrderResponse>(order);

        return Result.Success<OrderResponse>.Get(orderResponse);
    }
}