namespace Contract.Services.MonthlyCompanySalary.ShareDtos;

public record MaterialExportReponse
(
    Guid MaterialId,
    string MaterialName,
    string MaterialUnit,
    string MaterialImage,
    double Quantity,
    decimal Price
    );
