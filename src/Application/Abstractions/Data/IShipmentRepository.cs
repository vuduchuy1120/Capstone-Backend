using Contract.Services.Shipment.GetShipments;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IShipmentRepository
{
    void Add(Shipment shipment);
    Task<Shipment> GetByIdAsync(Guid shipmentId);
    void Update(Shipment shipment);
    Task<bool> IsShipmentIdExistAsync(Guid shipmentId);
    Task<(List<Shipment>, int)> SearchShipmentAsync(GetShipmentsQuery request);
    Task<Shipment> GetByIdAndShipmentDetailAsync(Guid shipmentId);
}
