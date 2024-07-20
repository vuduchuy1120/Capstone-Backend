using Application.Abstractions.Data;
using Contract.Services.ProductPhase.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.ProductPhases.Updates;

public sealed class UpdateQuantityPhaseRequestValidator : AbstractValidator<UpdateQuantityPhaseRequest>
{
    public UpdateQuantityPhaseRequestValidator(
        ICompanyRepository _companyRepository,
        IPhaseRepository _phaseRepository,
        IProductRepository _productRepository,
        IProductPhaseRepository _productPhaseRepository
        )
    {
        RuleFor(x => x.quantity)
            .NotEmpty()
            .WithMessage("Bạn cần nhập số lượng hàng đã hoàn thiện")
            .GreaterThan(0)
            .WithMessage("Số lượng hàng hoàn thiện phải lớn hơn 0");
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("CompanyId là bắt buộc")
            .MustAsync(async (companyId, _) =>
            {
                return await _companyRepository.IsCompanyMainFactory(companyId);
            }).WithMessage("CompanyId không tìm thấy hoặc không phải trụ sở chính");
        RuleFor(x => x.PhaseId)
            .NotEmpty()
            .WithMessage("PhaseId là bắt buộc")
            .MustAsync(async (phaseId, _) =>
            {
                return await _phaseRepository.IsPhase2(phaseId);
            }).WithMessage("PhaseId không tìm thấy");
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId là bắt buộc")
            .MustAsync(async (productId, _) =>
            {
                return await _productRepository.IsProductIdExist(productId);
            }).WithMessage("ProductId không tìm thấy");
        RuleFor(pp => new { pp.ProductId, pp.PhaseId, pp.CompanyId })
            .MustAsync(async (pp, _) =>
            {
                return await _productPhaseRepository.IsProductPhaseExist(pp.ProductId, pp.PhaseId, pp.CompanyId);
            }).WithMessage("ProductPhase không tìm thấy");
    }
}
