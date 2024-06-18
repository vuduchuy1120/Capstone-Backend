namespace Contract.Services.ProductPhase.ShareDto;

public record ProductPhaseResponse
(
    Guid ProductId,
    Guid PhaseId,
    string ProductName,
    string PhaseName,
    int Quantity,
    decimal SalaryPerProduct
    );
