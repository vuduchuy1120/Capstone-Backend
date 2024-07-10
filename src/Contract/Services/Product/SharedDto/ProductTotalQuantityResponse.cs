namespace Contract.Services.Product.SharedDto;

public record ProductTotalQuantityResponse
(
    Guid PhaseId,
    string PhaseName,
    string PhaseDescription,
    int Quantity
    );