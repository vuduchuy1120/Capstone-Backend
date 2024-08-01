using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.SharedDto;
using Contract.Services.Product.UpdateProduct;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using Domain.Exceptions.ProductPhaseSalaries;
using Domain.Exceptions.Products;
using FluentValidation;

namespace Application.UserCases.Commands.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler(
    IProductRepository _productRepository,
    IProductImageRepository _productImageRepository,
    IPhaseRepository _phaseRepository,
    IProductPhaseSalaryRepository _productPhaseSalaryRepository,
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

        var isImageValid = IsProductImageAfterUpdateValid(product);
        if(!isImageValid)
        {
            throw new ProductBadRequestException("Sản phẩm phải có 1 ảnh chính");
        }

        _productRepository.Update(product);

        await UpdateProductPhaseSalaries(productId, updateProductRequest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success.Update();
    }

    private bool IsProductImageAfterUpdateValid(Product product)
    {
        return product.Images.Count(image => image.IsMainImage) == 1;
    }

    private async Task UpdateProductPhaseSalaries(Guid productId, UpdateProductRequest updateProductRequest)
    {
        // Get all phases
        var phases = await _phaseRepository.GetPhases();

        //Get phase ids
        var phase1 = phases.FirstOrDefault(x => x.Name == "PH_001").Id;
        var phase2 = phases.FirstOrDefault(x => x.Name == "PH_002").Id;
        var phase3 = phases.FirstOrDefault(x => x.Name == "PH_003").Id;

        var productPhaseSalary1 = await _productPhaseSalaryRepository.GetByProductIdAndPhaseId(productId, phase1)
            ?? throw new ProductPhaseSalaryNotFoundException();
        var productPhaseSalary2 = await _productPhaseSalaryRepository.GetByProductIdAndPhaseId(productId, phase2)
            ?? throw new ProductPhaseSalaryNotFoundException();
        var productPhaseSalary3 = await _productPhaseSalaryRepository.GetByProductIdAndPhaseId(productId, phase3)
            ?? throw new ProductPhaseSalaryNotFoundException();

        // Add product phase salaries
        productPhaseSalary1.Update(productId, phase1, updateProductRequest.PricePhase1);
        productPhaseSalary2.Update(productId, phase2, updateProductRequest.PricePhase2);
        productPhaseSalary3.Update(productId, phase3, updateProductRequest.PriceFinished);

        var productPhaseSalaries = new List<ProductPhaseSalary>
        {
            productPhaseSalary1,
            productPhaseSalary2,
            productPhaseSalary3
        };

        _productPhaseSalaryRepository.UpdateRange(productPhaseSalaries);
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
