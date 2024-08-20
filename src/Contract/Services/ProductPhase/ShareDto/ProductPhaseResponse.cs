namespace Contract.Services.ProductPhase.ShareDto;

public record ProductPhaseResponse
(
    Guid ProductId,
    Guid PhaseId,
    string ProductName,
    string PhaseName,
    int Quantity
    );

public record ProductPhaseWithCompanyResponse
(
    Guid CompanyId,
    string CompanyName,
    List<QuantityProductPhaseResponse> QuantityProductPhases
    );

public record QuantityProductPhaseResponse
(
    Guid PhaseId,
    string PhaseName,
    int Quantity
    );


public record SearchProductPhaseResponse(
    Guid CompanyId,
    string CompanyName,
    Guid ProductId,
    string ProductName,
    string ProductCode,
    string ImageUrl,
    Guid PhaseId,
    string PhaseName,
    string PhaseDescription,
    int ErrorAvailableQuantity,
    int AvailableQuantity,
    int BrokenAvailableQuantity,
    int FailureAvailabeQuantity
    );
