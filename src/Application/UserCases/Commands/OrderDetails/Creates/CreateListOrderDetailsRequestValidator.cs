using Application.Abstractions.Data;
using Contract.Services.OrderDetail.Creates;
using FluentValidation;

namespace Application.UserCases.Commands.OrderDetails.Creates;

public sealed class CreateListOrderDetailsRequestValidator : AbstractValidator<CreateListOrderDetailsRequest>
{
    public CreateListOrderDetailsRequestValidator(
        IOrderRepository _orderRepository,
        IOrderDetailRepository _orderDetailRepository,
        IProductRepository _productRepository,
        ISetRepository _setRepository)
    {
        RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required.")
                .MustAsync(async (orderId, cancellationToken) =>
                {
                    // Check if OrderId exists in the database
                    return await _orderRepository.IsOrderExist(orderId);
                }).WithMessage("OrderId does not exist.");

        RuleFor(x => x.OrderDetailRequests)
            .NotEmpty().WithMessage("OrderDetailRequests is required.")
            .Must(x => x.Count > 0).WithMessage("OrderDetailRequests is required.")
            .MustAsync(async (request, orderDetailRequests, cancellationToken) =>
            {
                // Get all ProductIds and SetIds from OrderDetailRequests
                var productIds = orderDetailRequests.Select(o => o.ProductId).Where(p => p.HasValue).Distinct().ToList();
                var setIds = orderDetailRequests.Select(o => o.SetId).Where(s => s.HasValue).Distinct().ToList();

                // Check existence of ProductIds and SetIds in one call
                //var productCheckTask = _productRepository.IsAllProductIdsExistAsync(productIds);
                var setCheckTask = _setRepository.IsAllSetIdExistAsync(setIds);

                // Await both tasks
                //var productExists = await productCheckTask;
                var setExists = await setCheckTask;
                //productExists &&
                return  setExists;
            }).WithMessage("ProductId or SetId does not exist.");

        RuleForEach(x => x.OrderDetailRequests)
            .Must((request, orderDetailRequest) =>
            {
                // productId or SetId must be provided and only one of them can be non-null
                return (orderDetailRequest.ProductId.HasValue && !orderDetailRequest.SetId.HasValue) ||
                       (!orderDetailRequest.ProductId.HasValue && orderDetailRequest.SetId.HasValue);
            }).WithMessage("Exactly one of ProductId or SetId must be provided.")
            .Must((request, orderDetailRequest) =>
            {
                // Ensure productId and SetId are not both provided
                return !(orderDetailRequest.ProductId.HasValue && orderDetailRequest.SetId.HasValue);
            }).WithMessage("ProductId and SetId cannot be provided at the same time.");
    }
}
