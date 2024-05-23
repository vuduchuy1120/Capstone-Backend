
using Domain.Users;

namespace Application.Abstractions.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
