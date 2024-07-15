using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Product.GetProducts;
using Contract.Services.Product.SharedDto;
using Domain.Exceptions.Products;

namespace Application.UserCases.Queries.Products.GetProducts;

internal sealed class GetProductsQueryHandler(IProductRepository _productRepository, IMapper _mapper)
    : IQueryHandler<GetProductsQuery, SearchResponse<List<ProductResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ProductResponse>>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _productRepository.SearchProductAsync(request);

        var products = result.Item1;
        var totalPage = result.Item2;

        if (products is null || products.Count <= 0 || totalPage <= 0)
        {
            throw new ProductNotFoundException();
        }

        var data = products.ConvertAll(p => new ProductResponse(
            p.Id,
            p.Name,
            p.Code,
            p.Price,
            p.Size,
            p.Description,
            p.IsInProcessing,
            p.Images.Select(image => new ImageResponse(
                image.Id,
                image.ImageUrl,
                image.IsBluePrint,
                image.IsMainImage
            )).ToList()
        ));

        var searchResponse = new SearchResponse<List<ProductResponse>>(request.PageIndex, totalPage, data);

        return Result.Success<SearchResponse<List<ProductResponse>>>.Get(searchResponse);
    }
}
