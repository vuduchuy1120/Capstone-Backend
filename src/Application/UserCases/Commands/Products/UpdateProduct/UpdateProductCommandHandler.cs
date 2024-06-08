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
    //IProductUnitRepository _productUnitRepository,
    IProductImageRepository _productImageRepository,
    IUnitOfWork _unitOfWork,
    IValidator<UpdateProductRequest> _validator) : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result.Success> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var updateProductRequest = request.UpdateProductRequest;
        var productId = request.ProductId;

        var product = await FindAndValidateRequest(updateProductRequest, productId);

        //await RemoveOldProductImages(updateProductRequest?.Remove?.ImageIds);
        
        //await RemoveOldProductUnits(request.UpdateProductRequest.Remove.SubProductIds, productId);
        
        //AddNewProductImages(request.UpdateProductRequest.Add.Images, productId);
        
        //AddNewProductUnits(request.UpdateProductRequest.Add.ProductUnits, productId);

        product.Update(updateProductRequest, request.UpdatedBy);
        _productRepository.Update(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Update();
    }

    private async Task<Product> FindAndValidateRequest(UpdateProductRequest updateProductRequest, Guid productId)
    {
        if (productId != updateProductRequest.Id)
        {
            throw new ProductIdConflictException();
        }

        var validationResult = _validator.Validate(updateProductRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }

        var product = await _productRepository.GetProductById(productId)
            ?? throw new ProductNotFoundException();
        var code = updateProductRequest.Code;
        var isCodeExist = await _productRepository.IsProductCodeExist(code);

        if(isCodeExist && product.Code != code)
        {
            throw new ProductCodeAlreadyExistException(code);
        }

        return product;
    }

    //private async Task RemoveOldProductImages(List<Guid>? imageIds)
    //{
    //    if (imageIds is null || imageIds.Count == 0)
    //    {
    //        return;
    //    }

    //    var productImages = await _productImageRepository.GetProductImageIdsAsync(imageIds);
    //    if (productImages is not null && productImages.Count > 0)
    //    {
    //        _productImageRepository.DeleteRange(productImages);
    //    }
    //}

    //private async Task RemoveOldProductUnits(List<Guid>? subProductIds, Guid productId)
    //{
    //    if (subProductIds is null || subProductIds.Count == 0)
    //    {
    //        return;
    //    }

    //    var productUnits = await _productUnitRepository.GetBySubProductIdsAsync(productId, subProductIds);
    //    if(productUnits is not null  && productUnits.Count > 0)
    //    {
    //        _productUnitRepository.DeleteRange(productUnits);
    //    }
    //}

    //private void AddNewProductImages(List<ImageRequest>? addProductImagesRequest, Guid productId)
    //{
    //    var productImages = addProductImagesRequest?
    //        .Select(imageRequest => ProductImage.Create(productId, imageRequest));
    //    if (productImages != null)
    //    {
    //        _productImageRepository.AddRange(productImages.ToList());
    //    }
    //}

    //private void AddNewProductUnits(List<ProductUnitRequest>? productUnitsRequest, Guid productId)
    //{
    //    var productUnits = productUnitsRequest?
    //        .Select(request => ProductUnit.Create(productId, request.SubProductId, request.QuantityPerUnit));
    //    if (productUnits is not null)
    //    {
    //        _productUnitRepository.AddRange(productUnits.ToList());
    //    }
    //}
}
