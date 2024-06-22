using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Order.ShareDtos;

namespace Contract.Services.Order.Queries;

public record SearchOrderQuery(
    string? CompanyName,
    string? Status,
    string? StartOrder,
    string? EndOrder,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQueryHandler<SearchResponse<List<OrderResponse>>>;