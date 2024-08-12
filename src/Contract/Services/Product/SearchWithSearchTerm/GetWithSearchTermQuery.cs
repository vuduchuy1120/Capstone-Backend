using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.SearchWithSearchTerm;

public record GetWithSearchTermQuery(
    string? SearchTerm,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<ProductWithOneImageResponse>>>;
