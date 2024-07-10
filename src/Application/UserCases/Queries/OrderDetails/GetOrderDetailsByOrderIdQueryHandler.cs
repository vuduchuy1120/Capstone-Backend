using Application.Abstractions.Data;
using Application.Abstractions.Services;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.OrderDetail.Queries;
using Contract.Services.OrderDetail.ShareDtos;
using Contract.Services.Product.SharedDto;
using Contract.Services.ProductPhaseSalary.ShareDtos;
using Domain.Exceptions.OrderDetails;

namespace Application.UserCases.Queries.OrderDetails
{
    public sealed class GetOrderDetailsByOrderIdQueryHandler : IQueryHandler<GetOrderDetailsByOrderIdQuery, OrderDetailResponse>
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICloudStorage _cloudStorage;

        public GetOrderDetailsByOrderIdQueryHandler(IOrderDetailRepository orderDetailRepository, ICloudStorage cloudStorage)
        {
            _orderDetailRepository = orderDetailRepository;
            _cloudStorage = cloudStorage;
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
                    var imageUrl = orderDetail.Product.Images.FirstOrDefault(x => x.IsMainImage)?.ImageUrl ?? string.Empty;
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        imageUrl = await _cloudStorage.GetSignedUrlAsync(imageUrl);
                    }

                    products.Add(new ProductOrderResponse(
                        orderDetail.Product.Id,
                        orderDetail.Product.Code,
                        orderDetail.Product.Name,
                        orderDetail.Product.Description,
                        imageUrl,
                        orderDetail.Quantity,
                        orderDetail.UnitPrice,
                        orderDetail.Note
                    ));
                }
                else if (orderDetail.SetId != null && orderDetail.Set != null)
                {
                    var setImageUrl = orderDetail.Set.ImageUrl;
                    if (!string.IsNullOrEmpty(setImageUrl))
                    {
                        setImageUrl = await _cloudStorage.GetSignedUrlAsync(setImageUrl);
                    }

                    var productResponses = await Task.WhenAll(orderDetail.Set.SetProducts.Select(async sp =>
                    {
                        var productImageUrls = await Task.WhenAll(sp.Product.Images.Select(async img =>
                        {
                            var signedUrl = await _cloudStorage.GetSignedUrlAsync(img.ImageUrl);
                            return new ImageResponse(img.Id, signedUrl, img.IsBluePrint, img.IsMainImage);
                        }));

                        return new ProductResponse(
                            sp.Product.Id,
                            sp.Product.Name,
                            sp.Product.Code,
                            sp.Product.Price,
                            sp.Product.ProductPhaseSalaries.Select(salary => new ProductPhaseSalaryResponse(
                                salary.PhaseId,
                                salary.Phase.Name,
                                salary.SalaryPerProduct
                            )).ToList(),
                            sp.Product.Size,
                            sp.Product.Description,
                            sp.Product.IsInProcessing,
                            productImageUrls.ToList()
                        );
                    }).ToList());

                    sets.Add(new SetOrderResponse(
                        orderDetail.Set.Id,
                        orderDetail.Set.Code,
                        orderDetail.Set.Name,
                        orderDetail.Set.Description,
                        setImageUrl,
                        productResponses.ToList(),
                        orderDetail.Quantity,
                        orderDetail.UnitPrice,
                        orderDetail.Note
                    ));
                }
            }

            var data = new OrderDetailResponse(
                OrderId: request.OrderId,
                ProductOrderResponses: products,
                SetOrderResponses: sets
            );

            return Result.Success<OrderDetailResponse>.Get(data);
        }
    }
}
