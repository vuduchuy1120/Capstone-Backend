using Contract.Abstractions.Messages;

namespace Contract.Services.User.BanUser;

public record ChangeUserStatusCommand(string updatedBy, string userId, bool isActive) : ICommand;
