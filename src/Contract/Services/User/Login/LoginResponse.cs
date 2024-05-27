using Contract.Services.User.SharedDto;

namespace Contract.Services.User.Login;

public record LoginResponse(UserResponse User, string AccessToken, string RefreshToken);
