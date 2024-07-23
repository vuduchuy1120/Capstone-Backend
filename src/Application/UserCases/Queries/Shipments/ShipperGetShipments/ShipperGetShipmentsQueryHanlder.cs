using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Shipment.Share;
using Contract.Services.Shipment.ShipperGetShipments;
using Domain.Exceptions.Shipments;

namespace Application.UserCases.Queries.Shipments.ShipperGetShipments;

internal sealed class ShipperGetShipmentsQueryHanlder(IShipmentRepository _shipmentRepository, IMapper _mapper)
    : IQueryHandler<ShipperGetShipmentsQuery, SearchResponse<List<ShipmentResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ShipmentResponse>>>> Handle(
        ShipperGetShipmentsQuery request, CancellationToken cancellationToken)
    {
        var (shipments, totalPages) = await _shipmentRepository.SearchShipmentOfShipperAsync(request.query, request.shipperId);

        if (shipments is null || shipments.Count == 0 || totalPages == 0)
        {
            throw new ShipmentNotFoundException();
        }
        var data = shipments.ConvertAll(s => _mapper.Map<ShipmentResponse>(s));

        var searchResponse = new SearchResponse<List<ShipmentResponse>>(request.query.PageIndex, totalPages, data);

        return Result.Success<SearchResponse<List<ShipmentResponse>>>.Get(searchResponse);
    }
}
