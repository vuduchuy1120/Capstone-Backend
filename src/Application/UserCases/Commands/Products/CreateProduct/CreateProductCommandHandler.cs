using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.CreateProduct;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.UserCases.Commands.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository _productRepository,
    IProductUnitRepository _productUnitRepository,
    IProductImageRepository _productImageRepository,
    IUnitOfWork _unitOfWork, 
    IValidator<CreateProductRequest> _validator) : ICommandHandler<CreateProductCommand>
{
    public async Task<Result.Success> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var createProductRequest = request.CreateProductRequest;

        await ValidateRequest(createProductRequest);

        var productId = AddProduct(createProductRequest, request.CreatedBy);
        AddProductUnits(createProductRequest.ProductUnitRequests, productId);
        AddProductImages(createProductRequest.ImageRequests, productId);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success.Create();
    }

    private async Task ValidateRequest(CreateProductRequest createProductRequest)
    {
        var validationResult = await _validator.ValidateAsync(createProductRequest);

        if (!validationResult.IsValid)
        {
            throw new MyValidationException(validationResult.ToDictionary());
        }
    }

    private Guid AddProduct(CreateProductRequest createProductRequest, string CreatedBy)
    {
        var product = Product.Create(createProductRequest, CreatedBy);
        _productRepository.Add(product);
        return product.Id;
    }

    private void AddProductUnits(List<ProductUnitRequest> productUnitRequests, Guid productId)
    {
        var productUnits = productUnitRequests.Select(productUnitRequest => ProductUnit.Create(
                productId,
                productUnitRequest.SubProductId,
                productUnitRequest.QuantityPerUnit))
            .ToList();

        _productUnitRepository.AddRange(productUnits);
    }

    private void AddProductImages(List<ImageRequest> imageRequests, Guid productId)
    {
        var productImages = imageRequests
            .Select(imageRequest => ProductImage.Create(productId, imageRequest))
            .ToList();
        
        _productImageRepository.AddRange(productImages);
    }
}
