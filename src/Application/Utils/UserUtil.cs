using Domain.Exceptions.Users;
using System.Security.Claims;

namespace Application.Utils;

public class UserUtil
{
    public static string GetUserIdFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst("UserID")?.Value ?? throw new UserDoNotLoggedInException();
    }
    public static string GetCompanyIdFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst("CompanyID")?.Value ?? throw new UserDoNotLoggedInException();
    }
    public static string GetRoleFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.FindFirst("Role")?.Value ?? throw new UserDoNotLoggedInException();
    }
}
