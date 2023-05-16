using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8600

namespace API.Data;
public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task UpdateSettings (int userId, SettingsDto settingsDto, string languageCode) 
    {
        var settings = await _context.Settings.FirstOrDefaultAsync(x => x.Id == userId);
        settings.MusicVolume = settingsDto.MusicVolume;
        settings.SoundVolume = settingsDto.SoundVolume;
        var langauge = await _context.Language.FirstOrDefaultAsync(x => x.Code == languageCode);
        settings.LanguageId = langauge.Id;
        await _context.SaveChangesAsync();
    }

    public async Task<MemberDto> GetMemberAsync(int id)
    {
        MemberDto memberDto = await _context.Users
            .Where(user => user.Id == id)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        
        Statistics userStatistics = await _context.Statistics.SingleOrDefaultAsync(x => x.AppUser.Id == memberDto.Id);
        memberDto.Statistics = _mapper.Map<StatisticsDto>(userStatistics);

        Settings userSettings = await _context.Settings.SingleOrDefaultAsync(x => x.AppUser.Id == memberDto.Id);
        memberDto.Settings = _mapper.Map<SettingsDto>(userSettings);

        Language userLanguage = await _context.Language.SingleOrDefaultAsync(x => x.Id == userSettings.LanguageId);
        memberDto.Language = _mapper.Map<LanguageDto>(userLanguage);

        return memberDto;
    }

    public async Task StatisticsIncrementOpenedChests(int userId)
    {
        var statistics = await GetStatistics(userId);
        statistics.OpenedChests++;
        await _context.SaveChangesAsync();
    }

    private async Task<Statistics> GetStatistics(int userId)
    {
        var statistics = await _context.Statistics.FirstOrDefaultAsync(x => x.Id == userId) ?? 
            throw new Exception($"No statistics found for user with id {userId}");
        return statistics;
    }
    
    public async Task StatisticsIncrementDefeatedEnemies(int userId)
    {
        var statistics = await GetStatistics(userId);
        statistics.DefeatedEnemies++;
        await _context.SaveChangesAsync();
    }
    
    public async Task StatisticsIncrementGamesPlayed(int userId)
    {
        var statistics = await GetStatistics(userId);
        statistics.GamesPlayed++;
        await _context.SaveChangesAsync();
    }
    
    public async Task StatisticsIncrementGamesWon(int userId)
    {
        var statistics = await GetStatistics(userId);
        statistics.GamesWon++;
        await _context.SaveChangesAsync();
    }
}
