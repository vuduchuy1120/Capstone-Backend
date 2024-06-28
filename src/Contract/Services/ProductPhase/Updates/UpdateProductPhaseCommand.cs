using Contract.Abstractions.Messages;

namespace Contract.Services.ProductPhase.Updates;

public record UpdateProductPhaseCommand(UpdateProductPhaseRequest updateProductPhaseRequest) : ICommand;