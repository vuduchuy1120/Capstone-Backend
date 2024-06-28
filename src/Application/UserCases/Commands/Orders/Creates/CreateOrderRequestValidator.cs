using Application.Abstractions.Data;
using Contract.Services.Order.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.Orders.Creates;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator(ICompanyRepository _companyRepository)
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required.")
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return await _companyRepository.IsExistAsync(companyId);
            }).WithMessage("Company does not exist.");
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.");            
    }
}
