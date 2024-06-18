using Application.Abstractions.Data;
using Contract.Services.ProductPhase.Updates;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCases.Commands.ProductPhases.Updates;

public sealed class UpdateProductPhaseRequestValidator : AbstractValidator<UpdateProductPhaseRequest>
{
    public UpdateProductPhaseRequestValidator(
            IPhaseRepository _phaseRepository,
            IProductRepository _productRepository,
            IProductPhaseRepository _productPhaseRepository)
    {
        RuleFor(x => x.PhaseId)
            .NotEmpty()
            .WithMessage("PhaseId is required")
            .MustAsync(async (phaseId, _) =>
            {
                return await _phaseRepository.IsExistById(phaseId);
            }).WithMessage("PhaseId not found!");
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required")
            .MustAsync(async (productId, _) =>
            {
                return await _productRepository.IsProductIdExist(productId);
            }).WithMessage("ProductId not found!");
        RuleFor(x => new { x.ProductId, x.PhaseId })
               .MustAsync(async (x, _) =>
               {
                   return await _productPhaseRepository.IsProductPhaseExist(x.ProductId, x.PhaseId);
               }).WithMessage("ProductPhase not found!");
    }
}
