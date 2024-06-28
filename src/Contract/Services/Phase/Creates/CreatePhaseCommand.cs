using Contract.Abstractions.Messages;

namespace Contract.Services.Phase.Creates;

public record CreatePhaseCommand(CreatePhaseRequest createPhaseRequest) : ICommand;

