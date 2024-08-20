using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.ProductPhase.ShareDto;

namespace Contract.Services.ProductPhase.Queries;

public record SearchProductPhaseQuery
(
    string? SearchCompany,
    string? SearchProduct,
    string? SearchPhase,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<SearchProductPhaseResponse>>>;
