using Contract.Abstractions.Messages;

namespace Contract.Services.User.ForgetPassword;

public record ForgetPasswordCommand(string userId) : ICommand;