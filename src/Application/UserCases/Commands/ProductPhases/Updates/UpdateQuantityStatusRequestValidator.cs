using Application.Abstractions.Data;
using Contract.ProductPhases.Updates.ChangeQuantityStatus;
using FluentValidation;

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
        RuleFor(x => x.PhaseIdFrom)
            .MustAsync(async (phaseId, cancellationToken) =>
            {
                return await _phaseRepository.IsExistById(phaseId);
            })
            .WithMessage("PhaseIdFrom không tồn tại.");
        RuleFor(x => x.PhaseIdTo)
            .MustAsync(async (phaseId, cancellationToken) =>
            {
                return await _phaseRepository.IsExistById(phaseId);
            })
            .WithMessage("PhaseIdTo không tồn tại.");
        RuleFor(x => x.CompanyIdFrom)
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return await _companyRepository.IsCompanyFactoryExistAsync(companyId);
            }).WithMessage("Công ty không tồn tại.");
        RuleFor(x => x.CompanyIdTo)
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return await _companyRepository.IsCompanyFactoryExistAsync(companyId);
            }).WithMessage("Công ty không tồn tại.");
        RuleFor(x => new { x.ProductId, x.PhaseIdFrom, x.CompanyIdFrom })
            .MustAsync(async (x, cancellationToken) =>
            {
                return await _productPhaseRepository.IsProductPhaseExist(x.ProductId, x.PhaseIdFrom, x.CompanyIdFrom);
            })
            .WithMessage("ProductPhase không tồn tại.");
        RuleFor(x => new { x.From, x.To, x.PhaseIdFrom, x.PhaseIdTo, x.CompanyIdFrom, x.CompanyIdTo })
            .Must(x =>
            {
                if(x.CompanyIdFrom == x.CompanyIdTo)
                {
                    if (x.PhaseIdFrom == x.PhaseIdTo)
                    {
                        return x.From != x.To;
                    }                
                }
                return true;
            })
            .WithMessage("Trạng thái thay đổi của sản phẩm phải khác nhau.");


    }
}
