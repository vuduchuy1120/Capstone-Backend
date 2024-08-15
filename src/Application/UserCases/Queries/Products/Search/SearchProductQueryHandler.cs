using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Product.Search;
using Contract.Services.Product.SharedDto;

namespace Application.UserCases.Queries.Products.Search;

internal sealed class SearchProductQueryHandler(
    IProductRepository _productRepository,
    IProductPhaseRepository _productPhaseRepository,
    ICloudStorage _cloudStorage)
    : IQueryHandler<SearchProductQuery, SearchResponse<List<ProductWithQuantityInformation>>>
{
    public async Task<Result.Success<SearchResponse<List<ProductWithQuantityInformation>>>> Handle(
        SearchProductQuery request,
        CancellationToken cancellationToken)
    {
        var (productPhases, totalPages) = await _productPhaseRepository.SearchProductByPhaseAndCompanyAsync(request);
        if (productPhases == null || !productPhases.Any())
        {
            var searchResponse = new SearchResponse<List<ProductWithQuantityInformation>>(request.PageIndex, totalPages, null);
            return Result.Success<SearchResponse<List<ProductWithQuantityInformation>>>.Get(searchResponse);
        }

        var data = await Task.WhenAll(productPhases.Select(async p =>
        {
            string imageUrl = "Image_not_found";

            var images = p.Product.Images;
            if (images != null && images.Any())
            {
                var image = images.FirstOrDefault(i => i.IsMainImage) ?? images.FirstOrDefault();
                if (image != null)
                {
                    imageUrl = await _cloudStorage.GetSignedUrlAsync(image.ImageUrl);
                }
            }

            var product = p.Product;

        return new ProductWithQuantityInformation(
            product.Id,
            product.Name,
            product.Code,
            product.Price,
            product.Size,
            product.Description,
            product.IsInProcessing,
            imageUrl,
            p.PhaseId,
            p.CompanyId,
            p.Quantity,
            p.AvailableQuantity,
            p.ErrorQuantity,
            p.ErrorAvailableQuantity,
            p.FailureQuantity,
            p.FailureAvailabeQuantity,
            p.BrokenQuantity,
            p.BrokenAvailableQuantity);
        }));

        var successResult = new SearchResponse<List<ProductWithQuantityInformation>>(request.PageIndex, totalPages, data.ToList());
        return Result.Success<SearchResponse<List<ProductWithQuantityInformation>>>.Get(successResult);
    }

}