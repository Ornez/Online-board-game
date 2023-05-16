using System.Security.Claims;
using API.Data.Game;
using API.Modules.Game.Keys;

namespace API.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static string UserName(this ClaimsPrincipal user) 
    {
        return user?.Claims?.FirstOrDefault(c => c.Type == AllKeys.TokenClaimNames.USER_NAME)?.Value;
    }

    public static string Role(this ClaimsPrincipal user) 
    {
        return user?.Claims?.FirstOrDefault(c => c.Type == AllKeys.TokenClaimNames.ROLE)?.Value;
    }

    public static string Id(this ClaimsPrincipal user) 
    {
        return user?.Claims?.FirstOrDefault(c => c.Type == AllKeys.TokenClaimNames.ID)?.Value;
    }

    public static UserData UserData(this ClaimsPrincipal user) 
    {
        UserData userData = new(
            int.Parse(user?.Claims?.FirstOrDefault(c => c.Type == AllKeys.TokenClaimNames.ID)?.Value),
            user?.Claims?.FirstOrDefault(c => c.Type == AllKeys.TokenClaimNames.USER_NAME)?.Value,
            user?.Claims?.FirstOrDefault(c => c.Type == AllKeys.TokenClaimNames.ROLE)?.Value
        );
        return userData;
    }
}
