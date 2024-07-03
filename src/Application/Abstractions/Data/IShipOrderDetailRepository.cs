using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IShipOrderDetailRepository
{
    void AddRange(List<ShipOrderDetail> shipOrderDetails);
}
