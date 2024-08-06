using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Company.ShareDtos;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipOrder.GetShipOrdersOfShipper;
using Contract.Services.ShipOrder.Share;
using Domain.Entities;

namespace Application.UserCases.Queries.ShipOrders.GetShipOrdersByShipper;

internal sealed class GetShipOrdersByShipperIdQueryHandler(IShipOrderRepository _shipOrderRepository, IMapper _mapper)
    : IQueryHandler<GetShipOrdersByShipperIdQuery, SearchResponse<List<ShipOrderForShipperResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ShipOrderForShipperResponse>>>> Handle(
        GetShipOrdersByShipperIdQuery request, 
        CancellationToken cancellationToken)
    {
        var (shipOrders, totalPages) = await _shipOrderRepository.SearchShipOrderByShipperAsync(request);

        if(shipOrders is null || shipOrders.Count == 0)
        {
            var emptyResult = new SearchResponse<List<ShipOrderForShipperResponse>>(
                request.SearchOption.PageIndex, totalPages, new List<ShipOrderForShipperResponse>());
            return Result.Success<SearchResponse<List<ShipOrderForShipperResponse>>>.Get(emptyResult);
        }

        var shipOrderResponse = shipOrders.Select(s =>
        {
            var companyResponse = _mapper.Map<CompanyResponse>(s.Order.Company);
            return new ShipOrderForShipperResponse(
            s.Id, s.ShipDate, s.IsAccepted, s.Status, s.Status.GetDescription(), s.DeliveryMethod, s.DeliveryMethod.GetDescription(), companyResponse);
        }).ToList();
        var result = new SearchResponse<List<ShipOrderForShipperResponse>>(request.SearchOption.PageIndex, totalPages, shipOrderResponse);

        return Result.Success<SearchResponse<List<ShipOrderForShipperResponse>>>.Get(result);
    }
}
