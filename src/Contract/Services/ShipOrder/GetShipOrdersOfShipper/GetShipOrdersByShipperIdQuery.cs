using Contract.Abstractions.Messages;
using Contract.Abstractions.Shared.Search;

namespace Contract.Services.ShipOrder.GetShipOrdersOfShipper;

public record GetShipOrdersByShipperIdQuery(string ShipperId, SearchShipOrderOption SearchOption) 
    : IQuery<SearchResponse<List<ShipOrderForShipperResponse>>>;
