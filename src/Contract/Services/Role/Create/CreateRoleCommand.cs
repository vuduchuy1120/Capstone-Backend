using Contract.Abstractions.Messages;

namespace Contract.Services.Role.Create;

public record CreateRoleCommand(string RoleName, string Decription) : ICommand;