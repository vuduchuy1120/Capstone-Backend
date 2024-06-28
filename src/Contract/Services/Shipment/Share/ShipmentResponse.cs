using Contract.Services.Company.Shared;
using Contract.Services.Company.ShareDtos;

namespace Contract.Services.Shipment.Share;

public record ShipmentResponse(
    CompanyResponse From, 
    CompanyResponse To,
    DateTime ShipDate, 
    string StatusDescription,
    Status Status);