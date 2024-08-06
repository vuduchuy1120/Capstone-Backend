using Application.Abstractions.Data;
using Contract.Services.Order.ShareDtos;
using Contract.Services.Order.Updates;
using FluentValidation;

namespace Application.UserCases.Commands.Orders.Updates;

public sealed class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator(
        IOrderRepository _orderRepository,
        ICompanyRepository _companyRepository,
        IShipOrderRepository _shipOrderRepository)
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

        RuleFor(x => x.Status)
            .MustAsync(async (request, status,_) =>
            {
                var isExistShipOrderNotDone = await _shipOrderRepository.IsAnyShipOrderNotDone(request.OrderId);
                if (isExistShipOrderNotDone)
                {
                    return status == StatusOrder.INPROGRESS;
                }
                return true;
            }).WithMessage("Đơn hàng đang có shipment đang chờ vận chuyển hoặc đang vận chuyển nên không thể thay đổi trạng thái. ");

        RuleFor(x => x.Status)
            .MustAsync(async (request, status, _) =>
            {
                var isExistShipOrder = await _shipOrderRepository.IsExistAnyShipOrder(request.OrderId);
                if (isExistShipOrder)
                {
                    return status != StatusOrder.SIGNED;
                }
                return true;
            }).WithMessage("Đã có sản phẩm được giao đi không thể chuyển trạng thái đơn hàng về đã ký");

        RuleFor(x => x.CompanyId)
           .MustAsync(async (request, CompanyId, _) =>
           {
               var isExistShipOrder = await _shipOrderRepository.IsExistAnyShipOrder(request.OrderId);
               if (isExistShipOrder)
               {
                   return await _orderRepository.IsCompanyNotChange(request.OrderId, CompanyId);
               }
               return false;
           }).WithMessage("Đã có sản phẩm được giao đi không thể thay đổi công ty đặt hàng.");

        RuleFor(x => x.Status)
            .MustAsync(async (request, status, _) =>
            {
                var isOrderComplete = await _orderRepository.IsOrderComplete(request.OrderId);
                if (!isOrderComplete)
                {
                    return status != StatusOrder.COMPLETED;
                }
                return true;
            }).WithMessage("Đơn hàng có những sản phẩm chưa được giao hết không thể thay đổi trạng thái thành hoàn thành.");
        RuleFor(x => x.VAT)
            .Must(VAT =>
            {
                return VAT == 0 || VAT == 5 || VAT == 8 || VAT == 10;
            }).WithMessage("Thuế của đơn hàng chỉ nhận giá trị 0,5,8,10!");
    }
}
