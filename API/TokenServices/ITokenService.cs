namespace API.TokenServices;

public interface ITokenService
{
    string CreateToken(string userName, string role, int id);
}
