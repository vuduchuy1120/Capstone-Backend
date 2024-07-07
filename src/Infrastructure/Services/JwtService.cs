using Application.Abstractions.Services;
using Domain.Entities;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

internal class JwtService : IJwtService
{
    //TODO change 5 to 50
    private const int Access_Token_Time_In_Minutes = 5;
    private const int Refresh_Token_Time_In_Minutes = 10;

    private readonly JwtOptions _jwtOptions;

    public JwtService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<string> CreateAccessToken(User user)
    {
        return await GenerateToken(user, Access_Token_Time_In_Minutes);
    }

    public async Task<string> CreateRefreshToken(User user)
    {
        return await GenerateToken(user, Refresh_Token_Time_In_Minutes);
    }

    public string? GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserID");

        return userIdClaim?.Value;
    }

    private async Task<string> GenerateToken(User user, int time)
    {
        var claims = new Claim[]
        {
            new Claim("UserID", user.Id),
            new Claim("UserName", user.FirstName + " " + user.LastName),
            new Claim("Role", user.Role.RoleName),
            new Claim("CompanyID", user.CompanyId.ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            null,
            DateTime.Now.AddMinutes(time),
            signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return await Task.FromResult(tokenValue);
    }

}
