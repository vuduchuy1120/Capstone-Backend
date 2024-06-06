namespace Contract.Services.MaterialHistory.Create;

public record CreateMaterialHistoryRequest
(
    int MaterialId,
    double Quantity,
    double? QuantityPerUnit,
    decimal Price,
    string? Description,
    double? QuantityInStock,
    string ImportDate
    );

