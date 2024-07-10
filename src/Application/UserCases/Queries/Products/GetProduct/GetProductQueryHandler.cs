using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.GetProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.ProductPhase.ShareDto;
using Contract.Services.ProductPhaseSalary.ShareDtos;
using Domain.Exceptions.Products;

namespace Application.UserCases.Queries.Products.GetProduct;

internal sealed class GetProductQueryHandler(IProductRepository _productRepository, IMapper _mapper)
    : IQueryHandler<GetProductQuery, ProductWithTotalQuantityResponse>
{
    public async Task<Result.Success<ProductWithTotalQuantityResponse>> Handle(
        GetProductQuery request,
        CancellationToken cancellationToken)
    {
        var p = await _productRepository.GetProductByIdWithProductPhase(request.productId)
            ?? throw new ProductNotFoundException();

        var productResponse = new ProductWithTotalQuantityResponse(
            p.Id,
            p.Name,
            p.Code,
            p.Price,
            p.ProductPhaseSalaries.Select(salary => new ProductPhaseSalaryResponse(
                salary.PhaseId,
                salary.Phase.Name,
                salary.Phase.Description,
                salary.SalaryPerProduct
            )).ToList(),
            p.ProductPhases
            .GroupBy(phase => new { phase.PhaseId, phase.ProductId, phase.Phase.Name })
            .OrderByDescending(g => g.Key.Name)
            .Select(g => new ProductTotalQuantityResponse(
                g.Key.PhaseId,
                g.First().Phase.Name,
                g.First().Phase.Description,
                g.Sum(phase => phase.Quantity)
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

        return Result.Success<ProductWithTotalQuantityResponse>.Get(productResponse);
    }
}
