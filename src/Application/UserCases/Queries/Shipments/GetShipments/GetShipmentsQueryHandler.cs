using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Shipment.GetShipments;
using Contract.Services.Shipment.Share;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Queries.Shipments.GetShipments;

internal sealed class GetShipmentsQueryHandler(
    IShipmentRepository _shipmentRepository, IMapper _mapper)
    : IQueryHandler<GetShipmentsQuery, SearchResponse<List<ShipmentResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ShipmentResponse>>>> Handle(
        GetShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var (shipments, totalPages) = await _shipmentRepository.SearchShipmentAsync(request);

        List<ShipmentResponse> data = null;

        if(shipments is null || shipments.Count == 0 || totalPages == 0)
        {
            data = new List<ShipmentResponse>();
        }
        else
        {
            data = shipments.ConvertAll(s => _mapper.Map<ShipmentResponse>(s));
        }

        var searchResponse = new SearchResponse<List<ShipmentResponse>>(request.PageIndex, totalPages, data);

        return Result.Success<SearchResponse<List<ShipmentResponse>>>.Get(searchResponse);
    }
}
