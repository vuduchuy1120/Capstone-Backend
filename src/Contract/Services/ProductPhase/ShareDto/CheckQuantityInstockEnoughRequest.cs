namespace Contract.Services.ProductPhase.ShareDto;

public record CheckQuantityInstockEnoughRequest(
    Guid ProductId,
    Guid PhaseId,
    Guid FromCompanyId,
    int Quantity);
