using Contract.Abstractions.Messages;

namespace Contract.Services.Set.CreateSet;

public record CreateSetCommand(CreateSetRequest CreateSetRequest, string CreatedBy) : ICommand;