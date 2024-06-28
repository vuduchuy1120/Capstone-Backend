using Contract.Abstractions.Messages;
using Contract.Services.User.Login;

namespace Contract.Services.User.RefreshToken;

public record RefreshTokenCommand(string userId, string refreshToken) : ICommand<LoginResponse>;
