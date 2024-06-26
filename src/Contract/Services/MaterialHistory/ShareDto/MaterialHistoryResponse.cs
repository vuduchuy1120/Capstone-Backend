namespace Contract.Services.MaterialHistory.ShareDto;
public record MaterialHistoryResponse(
    Guid Id,
    int MaterialId,
    double Quantity,
    decimal Price,
    string? Description,
    string? Image,
    string MaterialName,
    string MaterialUnit,
    DateOnly ImportDate
    );