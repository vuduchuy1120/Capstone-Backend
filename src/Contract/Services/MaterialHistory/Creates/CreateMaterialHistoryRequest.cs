namespace Contract.Services.MaterialHistory.Create;

public record CreateMaterialHistoryRequest
(
    int MaterialId,
    double Quantity,
    decimal Price,
    string? Description,
    string ImportDate
    );

