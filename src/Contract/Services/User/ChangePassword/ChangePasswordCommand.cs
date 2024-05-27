using Contract.Abstractions.Messages;

namespace Contract.Services.User.ChangePassword;

public record ChangePasswordCommand(ChangePasswordRequest ChangePasswordRequest, string LoggedInUserId) : ICommand;
