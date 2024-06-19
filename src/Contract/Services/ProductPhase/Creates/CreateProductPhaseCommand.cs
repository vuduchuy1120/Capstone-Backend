using Contract.Abstractions.Messages;

namespace Contract.Services.ProductPhase.Creates;

public record CreateProductPhaseCommand(CreateProductPhaseRequest createProductPhaseRequest) : ICommand;