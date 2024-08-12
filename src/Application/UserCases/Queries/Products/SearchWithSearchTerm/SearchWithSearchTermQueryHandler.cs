using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Product.SearchWithSearchTerm;
using Contract.Services.Product.SharedDto;

namespace Application.UserCases.Queries.Products.SearchWithSearchTerm;

internal sealed class SearchWithSearchTermQueryHandler(IProductRepository _productRepository, ICloudStorage _cloudStorage)
    : IQueryHandler<GetWithSearchTermQuery, SearchResponse<List<ProductWithOneImageResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ProductWithOneImageResponse>>>> Handle(
        GetWithSearchTermQuery request, CancellationToken cancellationToken)
    {
        var (products, totalPages) = await _productRepository.SearchProductAsync(request);

        if(products.Count == 0)
        {
            var searchResponse = new SearchResponse<List<ProductWithOneImageResponse>>(request.PageIndex, totalPages, null);
            return Result.Success<SearchResponse<List<ProductWithOneImageResponse>>>.Get(searchResponse);
        }

        var data = await Task.WhenAll(products.Select(async p =>
        {
            string imageUrl = "Image_not_found";

            var images = p.Images;
            if (images != null && images.Any())
            {
                var image = images.FirstOrDefault(i => i.IsMainImage) ?? images.FirstOrDefault();
                if (image != null)
                {
                    imageUrl = await _cloudStorage.GetSignedUrlAsync(image.ImageUrl);
                }
            }

            return new ProductWithOneImageResponse(
            p.Id,
            p.Name,
            p.Code,
            p.Price,
            p.Size,
            p.Description,
            p.IsInProcessing,
            imageUrl);
        }));

        var result = new SearchResponse<List<ProductWithOneImageResponse>>(request.PageIndex, totalPages, data.ToList());
        return Result.Success<SearchResponse<List<ProductWithOneImageResponse>>>.Get(result);
    }
}
