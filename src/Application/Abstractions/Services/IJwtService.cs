
using Domain.Entities;

namespace Application.Abstractions.Services;

public interface IJwtService
{
    Task<string> CreateAccessToken(User user);
    Task<string> CreateRefreshToken(User user);
}
