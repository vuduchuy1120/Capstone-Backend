using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Product.SharedDto;

namespace Contract.Services.Product.GetProducts;

public record GetProductsQuery(
    string? SearchTerm,
    bool IsInProcessing = true,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<ProductResponseWithSalary>>>;
