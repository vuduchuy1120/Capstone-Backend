using Application.Abstractions.Data;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.OrderDetail.Queries;
using Contract.Services.OrderDetail.ShareDtos;
using Contract.Services.Product.SharedDto;
using Domain.Exceptions.OrderDetails;

namespace Application.UserCases.Queries.OrderDetails
{
    public sealed class GetOrderDetailsByOrderIdQueryHandler : IQueryHandler<GetOrderDetailsByOrderIdQuery, OrderDetailResponse>
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public GetOrderDetailsByOrderIdQueryHandler(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<Result.Success<OrderDetailResponse>> Handle(GetOrderDetailsByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(request.OrderId);

            if (orderDetails is null || orderDetails.Count <= 0)
            {
                throw new OrderDetailNotFoundException();
            }

            List<ProductOrderResponse> products = new List<ProductOrderResponse>();
            List<SetOrderResponse> sets = new List<SetOrderResponse>();

            foreach (var orderDetail in orderDetails)
            {
                if (orderDetail.ProductId != null && orderDetail.Product != null)
                {
                    products.Add(new ProductOrderResponse(
                        orderDetail.Product.Id,
                        orderDetail.Product.Code,
                        orderDetail.Product.Name,
                        orderDetail.Product.Description,
                        orderDetail.Product.Images.FirstOrDefault(x => x.IsMainImage)?.ImageUrl ?? string.Empty,
                        orderDetail.Quantity,
                        orderDetail.UnitPrice
                    ));
                }
                else if (orderDetail.SetId != null && orderDetail.Set != null)
                {
                    var productResponses = orderDetail.Set.SetProducts.Select(sp => new ProductResponse(
                        sp.Product.Id,
                        sp.Product.Name,
                        sp.Product.Code,
                        sp.Product.Price,
                        sp.Product.Size,
                        sp.Product.Description,
                        sp.Product.IsInProcessing,
                        sp.Product.Images.Select(img => new ImageResponse(img.Id, img.ImageUrl, img.IsBluePrint, img.IsMainImage)).ToList()
                    )).ToList();

                    sets.Add(new SetOrderResponse(
                        orderDetail.Set.Id,
                        orderDetail.Set.Name,
                        orderDetail.Set.Description,
                        orderDetail.Set.ImageUrl,
                        productResponses,
                        orderDetail.Quantity,
                        orderDetail.UnitPrice
                    ));
                }
            }

            var data = new OrderDetailResponse(
                OrderId: request.OrderId,
                ProductOrderResponses: products,
                SetOrderResponses: sets,
                Note: orderDetails.FirstOrDefault()?.Note ?? string.Empty
            );

            return Result.Success<OrderDetailResponse>.Get(data);
        }
    }
}
