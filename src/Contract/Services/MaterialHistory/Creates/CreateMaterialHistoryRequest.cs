namespace Contract.Services.MaterialHistory.Create;

public record CreateMaterialHistoryRequest
(
    Guid MaterialId,
    double Quantity,
    decimal Price,
    string? Description,
    string ImportDate
    );

