using Application.Abstractions.Data;
using Application.Utils;
using Contract.Abstractions.Shared.Utils;
using Contract.Services.Order.Creates;
using Contract.Services.Order.ShareDtos;
using FluentValidation;

namespace Application.UserCases.Commands.Orders.Creates;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator(
        ICompanyRepository _companyRepository,
        IProductRepository _productRepository,
        ISetRepository _setRepository)
    {
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

        RuleFor(x => x.VAT)
            .GreaterThanOrEqualTo(0).WithMessage("VAT phải lớn hơn hoặc bằng 0.");

        RuleFor(x => x.OrderDetailRequests)
            .NotEmpty().WithMessage("Chi tiết đơn hàng là bắt buộc.")
            .NotNull().WithMessage("Chi tiết đơn hàng không được bỏ trống.")
            .Must(x => x.Count > 0).WithMessage("Chi tiết đơn hàng là bắt buộc.");

        RuleFor(x=> x.StartOrder)
            .Must((request, startOrder) =>
            {
                if(request.Status == StatusOrder.COMPLETED || request.Status == StatusOrder.CANCELLED)
                    return DateUtil.BeLessThanCurrentDate(startOrder);
                return true;
            }).WithMessage("Ngày bắt đầu phải nhỏ hơn ngày hiện tại");
        RuleFor(x => x.OrderDetailRequests)
            .MustAsync(async (request, orderDetailRequests, cancellationToken) =>
            {
                List<Guid> productIds = new List<Guid>();
                List<Guid> setIds = new List<Guid>();
                foreach (var orderDetailRequest in orderDetailRequests)
                {
                    if (orderDetailRequest.isProductId)
                    {
                        productIds.Add(orderDetailRequest.ProductIdOrSetId);
                    }
                    else
                    {
                        setIds.Add(orderDetailRequest.ProductIdOrSetId);
                    }
                }
                var productIdsCheck = productIds.Distinct().ToList();
                var setIdsCheck = setIds.Distinct().ToList();

                bool productExists = true;
                bool setExists = true;

                if (productIds.Any())
                {
                    productExists = await _productRepository.IsAllProductIdsExistAsync(productIdsCheck);
                }
                if (setIds.Any())
                {
                    setExists = await _setRepository.IsAllSetIdExistAsync(setIds);
                }
                return productExists && setExists;
            }).WithMessage("Mã sản phẩm hoặc mã bộ sản phẩm không tồn tại.");

        RuleFor(x => x.OrderDetailRequests)
            .NotNull().WithMessage("Chi tiết đơn hàng không được bỏ trống.")
            .Must(orderDetailRequests =>
            {
                var distinctItems = orderDetailRequests
                    .Select(o => new { o.ProductIdOrSetId, o.isProductId })
                    .Distinct()
                    .Count();
                return distinctItems == orderDetailRequests.Count;
            }).WithMessage("Tìm thấy mã sản phẩm hoặc mã bộ sản phẩm trùng lặp trong chi tiết đơn hàng.");

        RuleForEach(x => x.OrderDetailRequests)
            .Must((request, orderDetailRequest) =>
            {
                // Số lượng phải lớn hơn 0
                return orderDetailRequest.Quantity > 0;
            }).WithMessage("Số lượng phải lớn hơn 0.");

        RuleForEach(x => x.OrderDetailRequests)
            .Must((request, orderDetailRequest) =>
            {
                // Đơn giá phải lớn hơn 0
                return orderDetailRequest.UnitPrice > 0;
            }).WithMessage("Đơn giá phải lớn hơn 0.");
    }
}
