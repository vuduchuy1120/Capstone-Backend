namespace Contract.Services.ProductPhase.Creates;

public record CreateProductPhaseRequest
(
    Guid ProductId,
    Guid PhaseId,
    int Quantity,
    Guid CompanyId
    );
