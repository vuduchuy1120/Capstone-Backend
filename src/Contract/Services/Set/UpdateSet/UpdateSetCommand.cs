using Contract.Abstractions.Messages;

namespace Contract.Services.Set.UpdateSet;

public record UpdateSetCommand(UpdateSetRequest UpdateSetRequest, string UpdatedBy, Guid setId) : ICommand;