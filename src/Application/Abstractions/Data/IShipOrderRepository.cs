using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IShipOrderRepository
{
    void Add(ShipOrder shipOrder);
    Task<List<ShipOrder>> GetByOrderIdAsync(Guid orderId);
}
