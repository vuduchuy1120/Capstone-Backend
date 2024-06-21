using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.ProductPhase.ShareDto;

namespace Contract.Services.ProductPhase.Queries;

public record GetProductPhasesQuery(
    int PageIndex = 1,
    int PageSize = 10)
    : IQueryHandler<SearchResponse<List<ProductPhaseResponse>>>;
