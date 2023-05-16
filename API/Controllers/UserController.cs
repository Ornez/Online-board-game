using API.DTOs;
using API.Extensions;
using API.Interfaces;
using API.Modules.Game.Keys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("settings")]
    public async Task<ActionResult<SettingsWithLanguageDto>> GetSettings()
    {
        if (User.Role() == AllKeys.Roles.GUEST)
        {
            return BadRequest($"{AllKeys.Roles.GUEST} role can't get settings.");
        }
        MemberDto member = await _userRepository.GetMemberAsync(int.Parse(User.Id()));
        return new SettingsWithLanguageDto(member.Settings.SoundVolume, 
            member.Settings.MusicVolume, member.Language.Code);
    }

    [HttpPut("settings")]
    public async Task<ActionResult> UpdateSettings(SettingsWithLanguageDto settings)
    {
        if (User.Role() == AllKeys.Roles.GUEST)
        {
            return BadRequest($"{AllKeys.Roles.GUEST} role can't update settings.");
        }

        try
        {
            settings.MusicVolume = Math.Clamp(settings.MusicVolume, 0, 100);
            settings.SoundVolume = Math.Clamp(settings.SoundVolume, 0, 100);

            await _userRepository.UpdateSettings(int.Parse(User.Id()), new(settings.SoundVolume, settings.MusicVolume), settings.LanguageCode);

            return Ok("Settings updated.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
        }
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<StatisticsDto>> GetStatistics()
    {
        if (User.Role() == AllKeys.Roles.GUEST)
        {
            return BadRequest($"{AllKeys.Roles.GUEST} role cant get statistics.");
        }

        MemberDto member = await _userRepository.GetMemberAsync(int.Parse(User.Id()));
        return member.Statistics;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDto>> GetProfile() 
    {
        if (User.Role() == AllKeys.Roles.GUEST)
        {
            return BadRequest($"{AllKeys.Roles.GUEST} role cant get profile.");
        }

        MemberDto member = await _userRepository.GetMemberAsync(int.Parse(User.Id()));
        return new ProfileDto(member.Email, member.Username);
    }
}
