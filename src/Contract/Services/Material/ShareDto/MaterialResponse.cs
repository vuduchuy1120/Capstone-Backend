namespace Contract.Services.Material.ShareDto;

public record MaterialResponse
(
    Guid Id,
    string Name,
    string? Description,
    string Unit,
    double? QuantityPerUnit,
    string? Image,
    double QuantityInStock
    );
