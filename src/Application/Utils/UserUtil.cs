using Domain.Exceptions.Users;
using System.Security.Claims;

namespace Application.Utils;

public class UserUtil
{
    public static string GetUserIdFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst("UserID")?.Value ?? throw new UserDoNotLoggedInException();
    }
}
