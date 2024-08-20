using Contract.Abstractions.Messages;

namespace Contract.Services.MaterialHistory.Deletes;

public record DeleteMaterialHistoryByIdCommand
(
    Guid MaterialHistoryId
    ): ICommand;
