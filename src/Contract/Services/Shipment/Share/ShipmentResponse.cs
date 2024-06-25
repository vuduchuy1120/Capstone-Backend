using Contract.Services.Company.Shared;

namespace Contract.Services.Shipment.Share;

public record ShipmentResponse(
    CompanyResponse From, 
    CompanyResponse To,
    DateTime ShipDate, 
    string StatusDescription,
    Status Status);