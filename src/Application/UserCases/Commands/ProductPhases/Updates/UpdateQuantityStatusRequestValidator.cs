using Application.Abstractions.Data;
using Contract.ProductPhases.Updates.ChangeQuantityStatus;
using Domain.Entities;
using FluentValidation;
using System.ComponentModel.Design;

namespace Application.UserCases.Commands.ProductPhases.Updates;

public sealed class UpdateQuantityStatusRequestValidator : AbstractValidator<UpdateQuantityStatusRequest>
{
    public UpdateQuantityStatusRequestValidator(
        IPhaseRepository _phaseRepository,
        IProductPhaseRepository _productPhaseRepository,
        ICompanyRepository _companyRepository,
        IProductRepository _productRepository)
    {
        RuleFor(x => x.From)
            .IsInEnum()
            .WithMessage("From must be a valid QuantityType 0,1,2,3");
        RuleFor(x => x.To)
            .IsInEnum()
            .WithMessage("To must be a valid QuantityType 0,1,2,3");
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
        RuleFor(x => x.ProductId)
            .MustAsync(async (productId, cancellationToken) =>
            {
                return await _productRepository.IsProductIdExist(productId);
            })
            .WithMessage("Product không tồn tại.");
        RuleFor(x => x.PhaseId)
            .MustAsync(async (phaseId, cancellationToken) =>
            {
                return await _phaseRepository.IsExistById(phaseId);
            })
            .WithMessage("Phase không tồn tại.");
        RuleFor(x => x.CompanyId)
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return await _companyRepository.IsCompanyFactoryExistAsync(companyId);
            }).WithMessage("Công ty không tồn tại.");
        RuleFor(x => new { x.ProductId, x.PhaseId, x.CompanyId })
            .MustAsync(async (x, cancellationToken) =>
            {
                return await _productPhaseRepository.IsProductPhaseExist(x.ProductId, x.PhaseId, x.CompanyId);
            })
            .WithMessage("ProductPhase không tồn tại.");
        RuleFor(x => new { x.From, x.To })
            .Must(x =>
            {
                return x.From != x.To;
            })
            .WithMessage("Trạng thái thay đổi của sản phẩm phải khác nhau.");


    }
}
