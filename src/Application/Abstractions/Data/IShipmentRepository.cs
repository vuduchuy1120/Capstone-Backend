using Contract.Services.Shipment.GetShipments;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Abstractions.Data;

public interface IShipmentRepository
{
    void Add(Shipment shipment);
    Task<Shipment> GetByIdAsync(Guid shipmentId);
    void Update(Shipment shipment);
    Task<bool> IsShipmentIdExistAndNotAcceptedAsync(Guid shipmentId);
    Task<(List<Shipment>, int)> SearchShipmentAsync(GetShipmentsQuery request);
    Task<Shipment> GetByIdAndShipmentDetailAsync(Guid shipmentId);
    Task<(List<Shipment>, int)> SearchShipmentOfShipperAsync(GetShipmentsQuery request, string shipperId);
    Task<Shipment> GetByIdAndShipperIdAsync(Guid shipmentId, string shipperId);
    Task<Shipment> GetShipmentDetailByIdAndShipperAsync(Guid shipmentId, string shipperId);

    Task<List<Shipment>> GetShipmentByCompanyIdAndMonthAndYearAsync(Guid CompanyId, int month, int year, bool received);
}
