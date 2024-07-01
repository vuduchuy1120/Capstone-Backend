using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.MaterialHistory.ShareDto;

namespace Contract.Services.MaterialHistory.Queries;

public record GetMaterialHistoriesByMaterialQuery(
    string? SearchTerms,
    string? StartDateImport,
    string? EndDateImport,
    int PageIndex = 1,
    int PageSize = 10
    ) : IQuery<SearchResponse<List<MaterialHistoryResponse>>>;