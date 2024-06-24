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
                .NotEmpty().WithMessage("OrderId is required.")
                .MustAsync(async (orderId, cancellationToken) =>
                {
                    return await orderRepository.IsOrderExist(orderId);
                }).WithMessage("OrderId does not exist.");

            RuleFor(x => x.OrderDetailRequests)
                .NotEmpty().WithMessage("OrderDetailRequests is required.")
                .Must(x => x.Count > 0).WithMessage("OrderDetailRequests is required.");

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
                }).WithMessage("ProductId or SetId does not exist.");
            RuleFor(x => x.OrderDetailRequests)
                .Must(orderDetailRequests =>
                {
                    var distinctItems = orderDetailRequests
                        .Select(o => new { o.ProductIdOrSetId, o.isProductId })
                        .Distinct()
                        .Count();
                    return distinctItems == orderDetailRequests.Count;
                }).WithMessage("Duplicate ProductIdOrSetId found in OrderDetailRequests.");

            RuleForEach(x => x.OrderDetailRequests)
                .Must((request, orderDetailRequest) =>
                {
                    // Quantity must be greater than 0
                    return orderDetailRequest.Quantity > 0;
                }).WithMessage("Quantity must be greater than 0.");

            RuleForEach(x => x.OrderDetailRequests)
                .Must((request, orderDetailRequest) =>
                {
                    // UnitPrice must be greater than 0
                    return orderDetailRequest.UnitPrice > 0;
                }).WithMessage("UnitPrice must be greater than 0.");            

        }
    }
}
