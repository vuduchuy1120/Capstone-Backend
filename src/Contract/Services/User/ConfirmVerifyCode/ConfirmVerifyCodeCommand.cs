using Contract.Abstractions.Messages;

namespace Contract.Services.User.ConfirmVerifyCode;

public record ConfirmVerifyCodeCommand(string UserId, string VerifyCode, string Password)
    : ICommand;