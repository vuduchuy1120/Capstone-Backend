namespace Contract.Services.Material.ShareDto;

public record MaterialResponse
(
    int Id,
    string Name,
    string? Description,
    string Unit,
    double? QuantityPerUnit,
    string? Image,
    double QuantityInStock
    );
