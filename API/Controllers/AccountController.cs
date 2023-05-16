using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.TokenServices;
using API.Modules.Game.Keys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("addLanguage")]
    public async Task AddLanguage(Language language) 
    {
        Language lang = new Language {Name = language.Name, Code = language.Code};
        _context.Language.Add(lang);
        await _context.SaveChangesAsync();
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
    {
        var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == registerDto.Username);

        if (user != null && user.UserName == registerDto.Username) 
        {
            return BadRequest("Username is taken.");
        }

        if (registerDto.Username.Length >= 5 && registerDto.Username.Substring(0, 5).ToLower() == "guest") 
        {
            return BadRequest("Username cannot start with 'guest'.");
        }

        using var hmac = new HMACSHA512();

        user = new AppUser 
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);

        var settings = new Settings 
        {
            SoundVolume = 5,
            MusicVolume = 5,
            Language = _context.Language.First(),
            AppUser = user
        };

        _context.Settings.Add(settings);

        var statistics = new Statistics 
        {
            GamesPlayed = 0,
            GamesWon = 0,
            DefeatedEnemies = 0,
            OpenedChests = 0,
            AppUser = user
        };

        _context.Statistics.Add(statistics);

        await _context.SaveChangesAsync();

        int id = _context.Users.SingleOrDefaultAsync(dbUser => dbUser.UserName == user.UserName).Id;

        return new UserDto 
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user.UserName, AllKeys.Roles.USER, id)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
    {
        var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == loginDto.Username);

        if (user == null || user.UserName != loginDto.Username) 
        {
            return Unauthorized("Invalid Username.");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++) 
        {
            if (computedHash[i] != user.PasswordHash[i]) 
            {
                return Unauthorized("Invalid password.");
            }
        }

        return new UserDto 
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user.UserName, AllKeys.Roles.USER, user.Id)
        };
    }
}
