using Application.Abstractions.Data;
using Contract.Services.OrderDetail.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.OrderDetails.Creates
{
    public sealed class CreateListOrderDetailsRequestValidator : AbstractValidator<CreateListOrderDetailsRequest>
    {
        public CreateListOrderDetailsRequestValidator(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IProductRepository productRepository,
            ISetRepository setRepository)
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Mã đơn hàng là bắt buộc.")
                .NotNull().WithMessage("Mã đơn hàng không được bỏ trống.")
                .MustAsync(async (orderId, cancellationToken) =>
                {
                    return await orderRepository.IsOrderExist(orderId);
                }).WithMessage("Mã đơn hàng không tồn tại.");

            RuleFor(x => x.OrderDetailRequests)
                .NotEmpty().WithMessage("Chi tiết đơn hàng là bắt buộc.")
                .NotNull().WithMessage("Chi tiết đơn hàng không được bỏ trống.")
                .Must(x => x.Count > 0).WithMessage("Chi tiết đơn hàng là bắt buộc.");

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
                        productExists = await productRepository.IsAllProductIdsExistAsync(productIdsCheck);
                    }
                    if (setIds.Any())
                    {
                        setExists = await setRepository.IsAllSetIdExistAsync(setIds);
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
}
