using Contract.Abstractions.Messages;

namespace Contract.ProductPhases.Updates.ChangeQuantityStatus;

public record UpdateQuantityStatusCommand
(
    UpdateQuantityStatusRequest updateQuantityStatusRequest
    ) : ICommand;
