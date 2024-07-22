namespace Contract.Services.ProductPhase.Updates;

public record UpdateQuantityPhaseRequest
(
    Guid ProductId,
    Guid PhaseIdFrom,
    Guid PhaseIdTo,
    Guid CompanyId,
    int quantity
    );