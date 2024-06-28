using Contract.Abstractions.Messages;

namespace Contract.Services.MaterialHistory.Create;

public record CreateMaterialHistoryCommand(CreateMaterialHistoryRequest createMaterialHistoryRequest) : ICommand;

