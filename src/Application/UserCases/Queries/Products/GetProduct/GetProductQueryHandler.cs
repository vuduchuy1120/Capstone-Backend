using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.GetProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.ProductPhaseSalary.ShareDtos;
using Domain.Exceptions.Products;

namespace Application.UserCases.Queries.Products.GetProduct;

internal sealed class GetProductQueryHandler(IProductRepository _productRepository, IMapper _mapper)
    : IQueryHandler<GetProductQuery, ProductWithSalaryResponse>
{
    public async Task<Result.Success<ProductWithSalaryResponse>> Handle(
        GetProductQuery request, 
        CancellationToken cancellationToken)
    {
        var p = await _productRepository.GetProductById(request.productId)
            ?? throw new ProductNotFoundException();

        var productResponse = new ProductWithSalaryResponse(
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
            p.Images.Select(image => new ImageResponse(
                image.Id,
                image.ImageUrl,
                image.IsBluePrint,
                image.IsMainImage
            )).ToList()
        );

        return Result.Success<ProductWithSalaryResponse>.Get(productResponse);
    }
}
