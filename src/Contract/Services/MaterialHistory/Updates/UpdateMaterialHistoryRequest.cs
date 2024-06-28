namespace Contract.Services.MaterialHistory.Update;

public record UpdateMaterialHistoryRequest
(
    Guid Id,
    int MaterialId,
    double Quantity,
    decimal Price,
    string? Description,
    string ImportDate
    );
