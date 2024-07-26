namespace Contract.Services.MonthlyCompanySalary.ShareDtos;

public record MaterialExportReponse
(
    Guid MaterialId,
    string MaterialName,
    string MaterialUnit,
    double Quantity,
    decimal Price
    );
