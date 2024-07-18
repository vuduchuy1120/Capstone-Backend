using Contract.Services.Company.ShareDtos;

namespace Contract.Services.Shipment.Share;

public record ShipmentResponse(
    Guid Id,
    CompanyResponse From, 
    CompanyResponse To,
    DateTime ShipDate, 
    string StatusDescription,
    Status Status);