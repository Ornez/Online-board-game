using API.Data.Token;
using API.Extensions;
using API.Modules.Game.Keys;
using API.TokenServices;
using API.Modules.Guests.Managers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TokenController : BaseApiController
{
    private readonly IGuestsManager guestsManager;
    private readonly ITokenService tokenService;

    public TokenController(ITokenService tokenService, IGuestsManager guestsManager)
    {
        this.tokenService = tokenService;
        this.guestsManager = guestsManager;
    }

    [HttpPost("getToken")]
    public ActionResult<TokenData> GetToken()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RefreshToken();

        return CreateNewGuest();
    }

    private TokenData RefreshToken()
    {
        if (User.Role() == AllKeys.Roles.GUEST)
            return RefreshGuestToken();

        return RefreshUserToken();
    }

    private TokenData RefreshGuestToken()
    {
        string token = HttpContext.Request.Headers["Authorization"].ToString().Substring(7);

        if (guestsManager.IsGuestRegistered(token))
            return new TokenData(token);
        
        return CreateNewGuest();
    }

    private TokenData CreateNewGuest()
    {
        var newGuest = guestsManager.CreateNewGuest();
        var newToken = tokenService.CreateToken(newGuest.Username, AllKeys.Roles.GUEST, newGuest.Id);
        guestsManager.RegisterGuest(newToken);
        return new TokenData(newToken);
    }

    private TokenData RefreshUserToken()
    {
        int.TryParse(User.Id(), out var userId);
        return new TokenData(tokenService.CreateToken(User.UserName(), AllKeys.Roles.USER, userId));
    }
}