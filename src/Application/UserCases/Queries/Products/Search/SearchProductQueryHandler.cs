using Application.Abstractions.Data;
using Application.Abstractions.Services;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.Search;
using Contract.Services.Product.SharedDto;

namespace Application.UserCases.Queries.Products.Search;

internal sealed class SearchProductQueryHandler(
    IProductRepository _productRepository,
    IMapper _mapper,
    ICloudStorage _cloudStorage)
    : IQueryHandler<SearchProductQuery, List<ProductWithOneImage>>
{
    public async Task<Result.Success<List<ProductWithOneImage>>> Handle(
        SearchProductQuery request, 
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchProductAsync(request.Search);
        if (products == null || !products.Any())
        {
            return Result.Success<List<ProductWithOneImage>>.Get(new List<ProductWithOneImage>());
        }

        var data = await Task.WhenAll(products.Select(async p =>
        {
            if(p.Images is null || p.Images.Count == 0)
            {
                throw new FileNotFoundException();
            }

            var image = p.Images.SingleOrDefault(i => i.IsMainImage)
                       ?? throw new FileNotFoundException();

            var url = await _cloudStorage.GetSignedUrlAsync(image.ImageUrl);

            return new ProductWithOneImage(p.Id, p.Name, p.Code, p.Price, p.Size,
                p.Description, p.IsInProcessing, url);
        }));

        return Result.Success<List<ProductWithOneImage>>.Get(data.ToList());
    }

}
