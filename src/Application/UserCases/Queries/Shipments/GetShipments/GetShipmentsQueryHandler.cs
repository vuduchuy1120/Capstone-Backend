using Application.Abstractions.Data;
using AutoMapper;
using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Results;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Shipment.GetShipments;
using Contract.Services.Shipment.Share;
using Contract.Services.User.SharedDto;
using Domain.Exceptions.Shipments;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.UserCases.Queries.Shipments.GetShipments;

internal sealed class GetShipmentsQueryHandler(
    IShipmentRepository _shipmentRepository, IMapper _mapper)
    : IQueryHandler<GetShipmentsQuery, SearchResponse<List<ShipmentResponse>>>
{
    public async Task<Result.Success<SearchResponse<List<ShipmentResponse>>>> Handle(
        GetShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentRepository.SearchShipmentAsync(request);

        var shipments = result.Item1;
        var totalPages = result.Item2;

        if(shipments is null || shipments.Count == 0 || totalPages == 0)
        {
            throw new ShipmentNotFoundException();
        }
        var data = shipments.ConvertAll(s => _mapper.Map<ShipmentResponse>(s));

        var searchResponse = new SearchResponse<List<ShipmentResponse>>(request.PageIndex, totalPages, data);

        return Result.Success<SearchResponse<List<ShipmentResponse>>>.Get(searchResponse);
    }
}
