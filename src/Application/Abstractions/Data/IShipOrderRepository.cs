using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IShipOrderRepository
{
    void Add(ShipOrder shipOrder);

}
