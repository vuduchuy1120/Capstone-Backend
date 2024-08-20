using Contract.Abstractions.Messages;

namespace Contract.Services.ProductPhase.Updates;

public record UpdateQuantityPhaseCommand
(
    UpdateQuantityPhaseRequest updateReq
    ) : ICommand;
