namespace Contract.Services.ProductPhase.Updates;

public record UpdateQuantityPhaseRequest
(
    Guid ProductId,
    Guid PhaseId,
    Guid CompanyId,
    int quantity
    );