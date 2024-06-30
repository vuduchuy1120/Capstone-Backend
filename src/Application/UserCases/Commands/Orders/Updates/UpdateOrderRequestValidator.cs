using Application.Abstractions.Data;
using Contract.Services.Order.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.Orders.Updates;

public sealed class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator(IOrderRepository _orderRepository, ICompanyRepository _companyRepository)
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.")
            .NotNull().WithMessage("OrderId must be not null.")
            .MustAsync(async (orderId, cancellationToken) =>
            {
                return await _orderRepository.IsOrderExist(orderId);
            }).WithMessage("Order does not exist.");
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId is required")
            .NotNull().WithMessage("CompanyId must be not")
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return await _companyRepository.IsExistAsync(companyId);
            }).WithMessage("Company does not exist.");
        RuleFor(x => x.CompanyId)
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return !await _companyRepository.IsCompanyNotCustomerCompanyAsync(companyId);
            }).WithMessage("Company is must be customer company.");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status is not valid. Status should be 0,1,2,3");
    }
}
