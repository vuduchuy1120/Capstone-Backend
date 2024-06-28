namespace Contract.Services.User.ConfirmVerifyCode;

public record ConfirmVerifyCodeRequest(string UserId, string VerifyCode, string Password);
