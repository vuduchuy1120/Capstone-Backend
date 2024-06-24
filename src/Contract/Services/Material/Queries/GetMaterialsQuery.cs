using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Material.ShareDto;

namespace Contract.Services.Material.Get;

public record GetMaterialsQuery
(
    string? SearchTerm,
    int PageIndex = 1,
    int PageSize = 10) : IQueryHandler<SearchResponse<List<MaterialResponse>>>;