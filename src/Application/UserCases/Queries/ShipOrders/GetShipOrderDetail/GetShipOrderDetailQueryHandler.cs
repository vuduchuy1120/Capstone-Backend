using Application.Abstractions.Data;
using Application.Abstractions.Services;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.ShareDtos;
using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;
using Contract.Services.Set.SharedDto;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.GetShipOrderByOrderId;
using Contract.Services.ShipOrder.GetShipOrderDetail;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;
using Domain.Exceptions.ShipOrder;
using System.Threading.Tasks;

namespace Application.UserCases.Queries.ShipOrders.GetShipOrderDetail;

internal sealed class GetShipOrderDetailQueryHandler(
    IShipOrderRepository _shipOrderRepository,
    IMapper _mapper, 
    ICloudStorage _cloudStorage)
    : IQueryHandler<GetShipOrderDetailQuery, DetailShipOrderResponse>
{
    public async Task<Result.Success<DetailShipOrderResponse>> Handle(GetShipOrderDetailQuery request, CancellationToken cancellationToken)
    {
        var shipOrder = await _shipOrderRepository.GetShipOrderDetailByShipOrderIdAsync(request.ShipOrderId);

        if(shipOrder == null)
        {
            throw new ShipOrderNotFoundException($"Không tìm thấy đơn giao có Id: {request.ShipOrderId}");
        }

        if (shipOrder.ShipOrderDetails == null)
        {
            throw new ShipOrderDetailNotFoundException();
        }

        var shipOrderDetailResponses = await GetShipOrderResponsesFromShipOrder(shipOrder);

        var companyResponse = _mapper.Map<CompanyResponse>(shipOrder.Order.Company);

        var shipOrderResponse = new DetailShipOrderResponse(
                shipOrder.Id,
                shipOrder.ShipperId,
                shipOrder.Shipper.FirstName + " " + shipOrder.Shipper.LastName,
                shipOrder.ShipDate,
                shipOrder.Status,
                shipOrder.Status.GetDescription(),
                shipOrder.DeliveryMethod,
                shipOrder.DeliveryMethod.GetDescription(),
                shipOrderDetailResponses,
                companyResponse);

        return Result.Success<DetailShipOrderResponse>.Get(shipOrderResponse);
    }

    private async Task<List<ShipOrderDetailWithImageLinkResponse>> GetShipOrderResponsesFromShipOrder(ShipOrder shipOrder)
    {
        var responses = await Task.WhenAll(shipOrder.ShipOrderDetails.Select(async shipOrderDetail =>
        {
            if (shipOrderDetail.Product != null)
            {
                var productResponse = await GetProductResponse(shipOrderDetail.Product);
                return new ShipOrderDetailWithImageLinkResponse(productResponse, null, shipOrderDetail.Quantity);
            }

            if (shipOrderDetail.Set != null)
            {
                var set = shipOrderDetail.Set;
                var imageUrl = await _cloudStorage.GetSignedUrlAsync(set.ImageUrl);

                var productResponses = await Task.WhenAll(set.SetProducts.Select(async sp => await GetProductResponse(sp.Product)));

                var setResponse = new SetWithProductOneImageResponse(set.Id, set.Code, set.Name, imageUrl, set.Description, productResponses.ToList());
                return new ShipOrderDetailWithImageLinkResponse(null, setResponse, shipOrderDetail.Quantity);
            }

            return null;
        }).ToList());

        return responses.Where(response => response != null).ToList();
    }


    private async Task<ProductWithOneImageResponse> GetProductResponse(Product product)
    {
        var image = await _cloudStorage.GetSignedUrlAsync(product.Images.FirstOrDefault(image => image.IsMainImage).ImageUrl);
        var productResonse = new ProductWithOneImageResponse(
            product.Id,
            product.Name,
            product.Code,
            product.Price,
            product.Size,
            product.Description,
            product.IsInProcessing,
            image);

        return productResonse;
    }

}
