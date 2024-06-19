using Application.Abstractions.Data;
using Contract.Services.ProductPhase.Creates;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.ProductPhases.Creates;

public sealed class CreateProductPhaseRequestValidator : AbstractValidator<CreateProductPhaseRequest>
{
    public CreateProductPhaseRequestValidator(
        IProductPhaseRepository _productPhaseRepository,
        IPhaseRepository _phaseRepository,
        IProductRepository _productRepository
    )
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required")
            .MustAsync(async (productId, cancellationToken) =>
            {
                return await _productRepository.IsProductIdExist(productId);
            })
            .WithMessage("ProductId not found");

        RuleFor(x => x.PhaseId)
            .NotEmpty()
            .WithMessage("PhaseId is required")
            .MustAsync(async (phaseId, cancellationToken) =>
            {
                return await _phaseRepository.IsExistById(phaseId);
            })
            .WithMessage("PhaseId not found");
        RuleFor(x => new { x.ProductId, x.PhaseId })
               .MustAsync(async (x, _) =>
               {
                   return !await _productPhaseRepository.IsProductPhaseExist(x.ProductId, x.PhaseId);
               }).WithMessage("ProductPhase already existed!");
        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be greater than or equal 0");

        RuleFor(x => x.SalaryPerProduct)
            .NotEmpty()
            .WithMessage("Price is required")
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");
    }
}
