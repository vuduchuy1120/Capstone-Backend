using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.SharedDto;
using Contract.Services.Product.UpdateProduct;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.Products;
using FluentValidation;

namespace Application.UserCases.Commands.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository _productRepository,
    IProductImageRepository _productImageRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateProductCommand> _validator) : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result.Success> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var updateProductRequest = request.UpdateProductRequest;
        var productId = request.ProductId;

        var product = await FindAndValidateRequest(request, productId);

        await RemoveOldProductImages(updateProductRequest?.RemoveImageIds);

        AddNewProductImages(updateProductRequest?.AddImagesRequest, productId);

        product.Update(updateProductRequest, request.UpdatedBy);
        _productRepository.Update(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Update();
    }

    private async Task<Product> FindAndValidateRequest(UpdateProductCommand request, Guid productId)
    {
        if (productId != request.UpdateProductRequest.Id)
        {
            throw new ProductIdConflictException();
        }

        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var product = await _productRepository.GetProductByIdWithoutImages(productId)
            ?? throw new ProductNotFoundException();
        var code = request.UpdateProductRequest.Code;
        var isCodeExist = await _productRepository.IsProductCodeExist(code);

        if(isCodeExist && product.Code != code)
        {
            throw new ProductCodeAlreadyExistException(code);
        }

        return product;
    }

    private async Task RemoveOldProductImages(List<Guid>? imageIds)
    {
        if (imageIds is null || imageIds.Count == 0)
        {
            return;
        }

        var productImages = await _productImageRepository.GetProductImageIdsAsync(imageIds);
        if (productImages is not null && productImages.Count > 0)
        {
            _productImageRepository.DeleteRange(productImages);
        }
    }

    private void AddNewProductImages(List<ImageRequest>? addProductImagesRequest, Guid productId)
    {
        var productImages = addProductImagesRequest?
            .Select(imageRequest => ProductImage.Create(productId, imageRequest));
        if (productImages != null)
        {
            _productImageRepository.AddRange(productImages.ToList());
        }
    }
}
