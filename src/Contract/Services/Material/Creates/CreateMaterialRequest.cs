
namespace Contract.Services.Material.Create;

public record CreateMaterialRequest
(
    string Name,
    string? Description,
    string Unit,
    double? QuantityPerUnit,
    string? Image
    );
