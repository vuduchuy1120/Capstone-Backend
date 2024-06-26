using Contract.Services.Company.Shared;
using Contract.Services.Shipment.Share;
using Contract.Services.ShipmentDetail.Share;
using Contract.Services.User.SharedDto;

namespace Contract.Services.Shipment.GetShipmentDetail;

public record ShipmentDetailResponse(
    CompanyResponse From,
    CompanyResponse To,
    UserResponse Shipper,
    DateTime ShipDate,
    string StatusDescription,
    Status Status,
    List<DetailResponse>? Details);
