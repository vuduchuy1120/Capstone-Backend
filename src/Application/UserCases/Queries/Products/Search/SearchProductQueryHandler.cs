using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.Search;
using Contract.Services.Product.SharedDto;

namespace Application.UserCases.Queries.Products.Search;

internal sealed class SearchProductQueryHandler(IProductRepository _productRepository, IMapper _mapper)
    : IQueryHandler<SearchProductQuery, List<ProductResponse>>
{
    public async Task<Result.Success<List<ProductResponse>>> Handle(
        SearchProductQuery request, 
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchProductAsync(request.Search);
        if(products == null)
        {
            return Result.Success<List<ProductResponse>>.Get(null);
        }

        var data = products.ConvertAll(p => _mapper.Map<ProductResponse>(p));

        return Result.Success<List<ProductResponse>>.Get(data);
    }
}
