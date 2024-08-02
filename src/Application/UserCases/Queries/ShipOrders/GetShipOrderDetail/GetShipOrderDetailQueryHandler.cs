using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Services.Company.ShareDtos;
using Contract.Services.Product.SharedDto;
using Contract.Services.Set.GetSet;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.GetShipOrderByOrderId;
using Contract.Services.ShipOrder.GetShipOrderDetail;
using Contract.Services.ShipOrder.Share;
using Domain.Exceptions.ShipOrder;

namespace Application.UserCases.Queries.ShipOrders.GetShipOrderDetail;

internal sealed class GetShipOrderDetailQueryHandler(IShipOrderRepository _shipOrderRepository, IMapper _mapper)
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

        var shipOrderDetailResponses = shipOrder.ShipOrderDetails.Select(detail =>
        {
            var product = detail.Product != null ? _mapper.Map<ProductResponse>(detail.Product) : null;
            var set = detail.Set != null ? _mapper.Map<SetResponse>(detail.Set) : null;
            return new ShipOrderDetailResponse(product, set, detail.Quantity);
        }).ToList();

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
}
