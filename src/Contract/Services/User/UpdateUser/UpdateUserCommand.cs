using Contract.Abstractions.Messages;

namespace Contract.Services.User.UpdateUser;

public record UpdateUserCommand(UpdateUserRequest UpdateUserRequest, string UpdatedBy) : ICommand;
