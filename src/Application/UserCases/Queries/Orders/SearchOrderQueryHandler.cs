using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Order.Queries;
using Contract.Services.Order.ShareDtos;
using Domain.Exceptions.Orders;

namespace Application.UserCases.Queries.Orders;

internal class SearchOrderQueryHandler
    (IOrderRepository _orderRepository, IMapper _mapper
    ) : IQueryHandler<SearchOrderQuery, SearchResponse<List<OrderResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<OrderResponse>>>> Handle(SearchOrderQuery request, CancellationToken cancellationToken)
    {
        var searchResult = await _orderRepository.SearchOrdersAsync(request);
        var orders = searchResult.Item1;
        var totalPage = searchResult.Item2;
        if (orders is null || orders.Count <= 0 || totalPage <= 0)
        {
            throw new OrderNotFoundException();
        }
        var data = orders.ConvertAll(order => _mapper.Map<OrderResponse>(order));
        var searchResponse = new SearchResponse<List<OrderResponse>>(request.PageIndex, totalPage, data);
        return Result.Success<SearchResponse<List<OrderResponse>>>.Get(searchResponse);

    }
}
