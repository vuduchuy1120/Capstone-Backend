namespace Contract.Services.MonthlyCompanySalary.ShareDtos;

public record ProductExportResponse
(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string ProductImage,
    Guid PhaseId,
    string PhaseName,
    double Quantity,
    decimal Price
    );
