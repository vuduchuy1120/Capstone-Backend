using Contract.Services.ShipmentDetail.Share;
using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IShipmentDetailRepository
{
    void AddRange(List<ShipmentDetail> shipmentDetail);
    Task<List<ShipmentDetail>> GetByShipmentIdAndIdsAsync(Guid shipmentId, List<Guid> shipDetailIds);
    Task<bool> IsAllShipDetailIdAndShipmentIdValidAsync(Guid shipmentId, List<Guid> shipDetailIds);
    void UpdateRange(List<ShipmentDetail> shipmentDetails);
    Task<List<ShipmentDetail>> GetShipmentDetailByShipmentIdAndProductPhaseType(Guid shipmentId, ProductPhaseType productPhaseType);
}
