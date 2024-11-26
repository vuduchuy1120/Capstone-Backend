using Contract.Services.Attendance.Create;
using Domain.Abstractions.Entities;

namespace Domain.Entities;

public class Token : EntityBase<int>
{
    public string UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public static Token Create(string userId, string accessToken, string refreshToken)
    {
        return new()
        {
            UserId = userId,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public void Update(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
