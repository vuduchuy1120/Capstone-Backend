namespace Contract.Services.Material.Update;

public record UpdateMaterialRequest
(
    Guid Id,
    string Name,
    string? Description,
    string Unit,
    double? QuantityPerUnit,
    string? Image,
    double QuantityInStock
    );
