using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Modules.Game.Keys;
using Microsoft.IdentityModel.Tokens;

namespace API.TokenServices;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config) 
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }
    
    public string CreateToken(string userName, string role, int id)
    {
        var claims = new List<Claim>
        {
            new Claim(AllKeys.TokenClaimNames.ID, id.ToString()),
            new Claim(AllKeys.TokenClaimNames.USER_NAME, userName),
            new Claim(AllKeys.TokenClaimNames.ROLE, role)
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor 
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
