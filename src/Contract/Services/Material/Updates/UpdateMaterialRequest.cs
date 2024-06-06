namespace Contract.Services.Material.Update;

public record UpdateMaterialRequest
(
    int Id,
    string Name,
    string? Description,
    string Unit,
    double? QuantityPerUnit,
    string? Image
    );
