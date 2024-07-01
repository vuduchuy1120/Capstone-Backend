using Application.Abstractions.Data;
using Contract.Services.Order.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.Orders.Updates;

public sealed class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator(IOrderRepository _orderRepository, ICompanyRepository _companyRepository)
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Mã đơn hàng là bắt buộc.")
            .NotNull().WithMessage("Mã đơn hàng không được bỏ trống.")
            .MustAsync(async (orderId, cancellationToken) =>
            {
                return await _orderRepository.IsOrderExist(orderId);
            }).WithMessage("Đơn hàng không tồn tại.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Mã công ty là bắt buộc.")
            .NotNull().WithMessage("Mã công ty không được bỏ trống.")
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return await _companyRepository.IsExistAsync(companyId);
            }).WithMessage("Công ty không tồn tại.");

        RuleFor(x => x.CompanyId)
            .MustAsync(async (companyId, cancellationToken) =>
            {
                return !await _companyRepository.IsCompanyNotCustomerCompanyAsync(companyId);
            }).WithMessage("Công ty phải là công ty khách hàng.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ. Trạng thái phải là 0, 1, 2 hoặc 3.");

        RuleFor(x => x.EndOrder)
            .GreaterThan(x => x.StartOrder).WithMessage("Ngày kết thúc phải lớn hơn ngày bắt đầu.");

        RuleFor(x => x.VAT)
            .GreaterThanOrEqualTo(0).WithMessage("VAT phải lớn hơn hoặc bằng 0.");
    }
}
