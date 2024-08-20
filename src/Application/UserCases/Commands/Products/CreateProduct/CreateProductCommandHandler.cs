using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Product.CreateProduct;
using Contract.Services.Product.SharedDto;
using Domain.Abstractions.Exceptions;
using Domain.Entities;
using FluentValidation;

namespace Application.UserCases.Commands.Products.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IProductRepository _productRepository,
    IProductImageRepository _productImageRepository,
    IProductPhaseSalaryRepository _productPhaseSalaryRepository,
    IPhaseRepository _phaseRepository,
    IUnitOfWork _unitOfWork,
    IValidator<CreateProductRequest> _validator) : ICommandHandler<CreateProductCommand>
{
    public async Task<Result.Success> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var createProductRequest = request.CreateProductRequest;

        await ValidateRequest(createProductRequest);

        var productId = AddProduct(createProductRequest, request.CreatedBy);
        AddProductImages(createProductRequest.ImageRequests, productId);

        await AddProductPhaseSalaries(productId, createProductRequest);

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

    private void AddProductImages(List<ImageRequest>? imageRequests, Guid productId)
    {
        var productImages = imageRequests?
            .Select(imageRequest => ProductImage.Create(productId, imageRequest))
            .ToList();

        if (productImages is null) return;

        _productImageRepository.AddRange(productImages);
    }

    private async Task AddProductPhaseSalaries(Guid productId, CreateProductRequest createProductRequest)
    {
        // Get all phases
        var phases = await _phaseRepository.GetPhases();

        //Get phase ids
        var phase1 = phases.FirstOrDefault(x => x.Name == "PH_001").Id;
        var phase2 = phases.FirstOrDefault(x => x.Name == "PH_002").Id;
        var phase3 = phases.FirstOrDefault(x => x.Name == "PH_003").Id;

        // Add product phase salaries
        var productPhaseSalaries = new List<ProductPhaseSalary>
            {
                ProductPhaseSalary.Create(productId, phase1, createProductRequest.PricePhase1),
                ProductPhaseSalary.Create(productId, phase2, createProductRequest.PricePhase2),
                ProductPhaseSalary.Create(productId, phase3, createProductRequest.PriceFinished)
            };

        _productPhaseSalaryRepository.AddRange(productPhaseSalaries);
    }
}
