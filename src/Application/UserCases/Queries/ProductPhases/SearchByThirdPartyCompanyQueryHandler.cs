using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.ProductPhase.SearchByThirdPartyCompany;

namespace Application.UserCases.Queries.ProductPhases;

internal sealed class SearchByThirdPartyCompanyQueryHandler(
    IProductPhaseRepository _productPhaseRepository,
    ICloudStorage _cloudStorage) : IQueryHandler<SearchByThirdPartyCompanyQuery, SearchResponse<List<ProductWithTotalQuantityResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ProductWithTotalQuantityResponse>>>> Handle(
        SearchByThirdPartyCompanyQuery request, 
        CancellationToken cancellationToken)
    {
        var (productPhases, totalPages) = await _productPhaseRepository.SearchByProductAndThirdPartyCompany(request);

        if(productPhases == null || productPhases.Count == 0)
        {
            var nullResult = new SearchResponse<List<ProductWithTotalQuantityResponse>>(request.PageIndex, 0, null);
            return Result.Success<SearchResponse<List<ProductWithTotalQuantityResponse>>>.Get(nullResult);
        }

        var data = await Task.WhenAll(productPhases.GroupBy(p => p.ProductId).Select(async p =>
        {
            string imageUrl = "Image_not_found";
            var product = p.First().Product;

            var images = product.Images;
            if (images != null && images.Any())
            {
                var image = images.FirstOrDefault(i => i.IsMainImage) ?? images.FirstOrDefault();
                if (image != null)
                {
                    imageUrl = await _cloudStorage.GetSignedUrlAsync(image.ImageUrl);
                }
            }

            var totalQuantity = 0;
            var totalAvailableQuantity = 0;

            foreach (var ph in p)
            {
                totalQuantity = totalQuantity + ph.Quantity + ph.FailureQuantity + ph.ErrorQuantity + ph.BrokenQuantity;
                totalAvailableQuantity = totalAvailableQuantity + ph.AvailableQuantity + ph.FailureAvailabeQuantity 
                                    + ph.ErrorAvailableQuantity + ph.BrokenAvailableQuantity;
            }
            
            return new ProductWithTotalQuantityResponse(
                product.Id,
                product.Name,
                product.Code,
                product.Price,
                product.Size,
                product.Description,
                product.IsInProcessing, 
                imageUrl, 
                totalQuantity, 
                totalAvailableQuantity);
        }));

        var result = new SearchResponse<List<ProductWithTotalQuantityResponse>>(request.PageIndex, 0, data.ToList());
        return Result.Success<SearchResponse<List<ProductWithTotalQuantityResponse>>>.Get(result);
    }
}
