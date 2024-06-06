using Contract.Abstractions.Messages;

namespace Contract.Services.MaterialHistory.Update;
public record UpdateMaterialHistoryCommand(UpdateMaterialHistoryRequest updateMaterialHistoryRequest) : ICommand;
