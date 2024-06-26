using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Shipment.Share;

namespace Contract.Services.Shipment.GetShipments;

public record GetShipmentsQuery(
    string? SearchTerm,
    Status Status,
    int PageIndex = 1,
    int PageSize = 10) : IQuery<SearchResponse<List<ShipmentResponse>>>;
