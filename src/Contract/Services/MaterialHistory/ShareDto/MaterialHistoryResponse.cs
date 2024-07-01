namespace Contract.Services.MaterialHistory.ShareDto;
public record MaterialHistoryResponse(
    Guid Id,
    Guid MaterialId,
    double Quantity,
    decimal Price,
    string? Description,
    DateOnly ImportDate,
    string? Image,
    string MaterialName,
    string MaterialUnit
    );