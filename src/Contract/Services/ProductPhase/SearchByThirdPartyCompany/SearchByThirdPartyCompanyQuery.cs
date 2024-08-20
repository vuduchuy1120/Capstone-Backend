using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;

namespace Contract.Services.ProductPhase.SearchByThirdPartyCompany;

public record SearchByThirdPartyCompanyQuery(
    string? Search,
    Guid CompanyId,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<ProductWithTotalQuantityResponse>>>;