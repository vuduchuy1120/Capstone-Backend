using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.Search;

public record SearchProductQuery(
    string? Search, 
    Guid PhaseId,
    Guid CompanyId, 
    int PageIndex = 1, 
    int PageSize = 10) : IQuery<SearchResponse<List<ProductWithQuantityInformation>>>;
