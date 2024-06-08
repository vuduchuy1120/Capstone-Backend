using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.GetProduct;
using Contract.Services.Product.SharedDto;
using Domain.Exceptions.Products;

namespace Application.UserCases.Queries.Products.GetProduct;

internal sealed class GetProductQueryHandler(IProductRepository _productRepository, IMapper _mapper)
    : IQueryHandler<GetProductQuery, ProductResponse>
{
    public async Task<Result.Success<ProductResponse>> Handle(
        GetProductQuery request, 
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductById(request.productId)
            ?? throw new ProductNotFoundException();

        var productResponse = _mapper.Map<ProductResponse>(product);
        
        return Result.Success<ProductResponse>.Get(productResponse);
    }
}
