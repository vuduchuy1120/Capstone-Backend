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
            .MustAsync(async (CompanyId, _) =>
            {
                return await _companyRepository.IsCompanyFactoryExistAsync(CompanyId);
            }).WithMessage("CompanyId không tìm thấy hoặc không phải trụ sở chính hoặc trụ sở phụ");

        RuleFor(x => x.PhaseIdFrom)
            .NotEmpty()
            .WithMessage("PhaseIdFrom là bắt buộc")
            .MustAsync(async (PhaseIdFrom, _) =>
            {
                return await _phaseRepository.IsExistById(PhaseIdFrom);

            }).WithMessage("PhaseIdTo không tìm thấy");
        RuleFor(x => x.PhaseIdTo)
            .NotEmpty()
            .WithMessage("PhaseIdTo là bắt buộc")
            .MustAsync(async (PhaseIdTo, _) =>
            {
                return await _phaseRepository.IsExistById(PhaseIdTo);
            }).WithMessage("PhaseIdTo không tìm thấy");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId là bắt buộc")
            .MustAsync(async (productId, _) =>
            {
                return await _productRepository.IsProductIdExist(productId);
            }).WithMessage("ProductId không tìm thấy");

        RuleFor(x => new { x.PhaseIdFrom, x.PhaseIdTo })
            .Must((x, _) =>
            {
                return x.PhaseIdFrom != x.PhaseIdTo;
            }).WithMessage("PhaseIdFrom và PhaseIdTo không được trùng nhau");
        RuleFor(pp => new { pp.ProductId, pp.PhaseIdFrom, pp.CompanyId })
            .MustAsync(async (pp, _) =>
            {
                return await _productPhaseRepository.IsProductPhaseExist(pp.ProductId, pp.PhaseIdFrom, pp.CompanyId);
            }).WithMessage("ProductPhase không tìm thấy");
    }
}
