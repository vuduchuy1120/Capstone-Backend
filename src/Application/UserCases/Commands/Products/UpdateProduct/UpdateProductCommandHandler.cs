using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.UpdateProduct;
using Domain.Abstractions.Exceptions;
using Domain.Exceptions.Products;
using FluentValidation;
using MediatR;

namespace Application.UserCases.Commands.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository _productRepository,
    IProductUnitRepository _productUnitRepository,
    IProductImageRepository _productImageRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateProductRequest> _validator) : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result.Success> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var updateProductRequest = request.UpdateProductRequest;
        await ValidateRequest(updateProductRequest, request.ProductId);

        // Remove old productImages
        var imageIds = updateProductRequest?.Remove?.ImageIds;
        if (imageIds is not null && imageIds.Count > 0)
        {
            var productImages = await _productImageRepository.GetProductImageIdsAsync(imageIds);
            if(productImages is not null && productImages.Count > 0)
            {
                _productImageRepository.DeleteRange(productImages);
            }
        }
        // Remove old productUnits
        var subProductIds = request.UpdateProductRequest.Remove.SubProductIds;
        if(subProductIds is not null && subProductIds.Count > 0)
        {

        }
        // Add new productImages
        // Add new productUnits
        // Update product

        return Result.Success.Update();
    }

    private async Task ValidateRequest(UpdateProductRequest udateProductRequest, Guid productId)
    {
        if (productId != udateProductRequest.Id)
        {
            throw new ProductIdConflictException();
        }

        var validationResult = await _validator.ValidateAsync(udateProductRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
    }

    
}
