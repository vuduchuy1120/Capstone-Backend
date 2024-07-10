using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.Search;
using Contract.Services.Product.SharedDto;
using Contract.Services.ProductPhaseSalary.ShareDtos;

namespace Application.UserCases.Queries.Products.Search;

internal sealed class SearchProductQueryHandler(
    IProductRepository _productRepository,
    ICloudStorage _cloudStorage)
    : IQueryHandler<SearchProductQuery, List<ProductWithOneImageWithSalary>>
{
    public async Task<Result.Success<List<ProductWithOneImageWithSalary>>> Handle(
        SearchProductQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchProductAsync(request.Search);
        if (products == null || !products.Any())
        {
            return Result.Success<List<ProductWithOneImageWithSalary>>.Get(new List<ProductWithOneImageWithSalary>());
        }

        var data = await Task.WhenAll(products.Select(async p =>
        {
            string imageUrl = "Image_not_found";
            
            if (p.Images != null && p.Images.Any())
            {
                var image = p.Images.FirstOrDefault(i => i.IsMainImage) ?? p.Images.FirstOrDefault();
                if (image != null)
                {
                    imageUrl = await _cloudStorage.GetSignedUrlAsync(image.ImageUrl);
                }
            }

            return new ProductWithOneImageWithSalary(
                p.Id,
                p.Name,
                p.Code,
                p.Price,
                p.ProductPhaseSalaries.Select(salary => new ProductPhaseSalaryResponse(
                    salary.PhaseId,
                    salary.Phase.Name,
                    salary.SalaryPerProduct
                )).ToList(),
                p.Size,
                p.Description,
                p.IsInProcessing,
                imageUrl
            );
        }));

        return Result.Success<List<ProductWithOneImageWithSalary>>.Get(data.ToList());
    }

}
