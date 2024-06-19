using Contract.Abstractions.Messages;

namespace Contract.Services.Phase.Updates;

public record UpdatePhaseCommand(UpdatePhaseRequest updatePhaseRequest) : ICommand;
