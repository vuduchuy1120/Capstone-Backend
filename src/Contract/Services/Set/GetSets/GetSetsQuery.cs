using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;

namespace Contract.Services.Set.GetSets;

public record GetSetsQuery(
    string? SearchTerm,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<SetsResponse>>>;