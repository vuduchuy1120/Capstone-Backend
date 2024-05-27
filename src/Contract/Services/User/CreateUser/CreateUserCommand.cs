using Contract.Abstractions.Messages;
using Contract.Services.User.CreateUser;

namespace Contract.Services.User.Command;

public record CreateUserCommand(CreateUserRequest CreateUserRequest ,string CreatedBy) : ICommand;
