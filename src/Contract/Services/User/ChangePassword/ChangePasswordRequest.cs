namespace Contract.Services.User.ChangePassword;

public record ChangePasswordRequest(string userId, string oldPassword, string newPassword);
