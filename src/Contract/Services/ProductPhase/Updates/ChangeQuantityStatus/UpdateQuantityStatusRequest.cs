
using Contract.Services.ProductPhase.ShareDto;

namespace Contract.ProductPhases.Updates.ChangeQuantityStatus;

public record UpdateQuantityStatusRequest
(
    QuantityType From,
    QuantityType To,
    int Quantity,
    Guid ProductId,
    Guid PhaseId,
    Guid CompanyId
    );
