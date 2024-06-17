namespace Contract.Services.MaterialHistory.ShareDto;
public record MaterialHistoryResponse(
    Guid Id,
    int MaterialId,
    double Quantity,
    double? QuantityPerUnit,
    decimal Price,
    string? Description,
    double? QuantityInStock,
    string? Image,
    string MaterialName,
    string MaterialUnit,
    DateOnly ImportDate
    );