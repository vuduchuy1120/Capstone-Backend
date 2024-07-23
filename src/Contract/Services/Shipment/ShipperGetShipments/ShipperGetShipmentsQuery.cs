using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;
using Contract.Services.Shipment.GetShipments;
using Contract.Services.Shipment.Share;

namespace Contract.Services.Shipment.ShipperGetShipments;

public record ShipperGetShipmentsQuery(
    string shipperId,
    GetShipmentsQuery query) : IQuery<SearchResponse<List<ShipmentResponse>>>;
