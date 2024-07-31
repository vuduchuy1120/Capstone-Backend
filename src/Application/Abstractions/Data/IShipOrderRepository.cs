using Contract.Services.ShipOrder.GetShipOrdersOfShipper;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IShipOrderRepository
{
    void Add(ShipOrder shipOrder);
    void Update(ShipOrder shipOrder);
    Task<List<ShipOrder>> GetByOrderIdAsync(Guid orderId);
    Task<bool> IsShipOrderExistAndInWaitingStatusAsync(Guid shipOrderId);
    Task<ShipOrder> GetByShipOrderIdAsync(Guid shipOrderId);
    Task<ShipOrder> GetByIdAndStatusIsNotDoneAsync(Guid shipOrderId);
    Task<(List<ShipOrder>, int)> SearchShipOrderByShipperAsync(GetShipOrdersByShipperIdQuery request);
}
