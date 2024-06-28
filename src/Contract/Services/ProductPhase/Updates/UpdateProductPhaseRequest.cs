namespace Contract.Services.ProductPhase.Updates;
public record UpdateProductPhaseRequest
(
    Guid ProductId,
    Guid PhaseId,
    int Quantity
    );


