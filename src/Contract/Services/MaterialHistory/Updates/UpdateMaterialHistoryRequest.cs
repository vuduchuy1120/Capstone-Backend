namespace Contract.Services.MaterialHistory.Update;

public record UpdateMaterialHistoryRequest
(
    Guid Id,
    int MaterialId,
    double Quantity,
    double? QuantityPerUnit,
    decimal Price,
    string? Description,
    double? QuantityInStock,
    string ImportDate
    );
