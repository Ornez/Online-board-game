using API.DTOs;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task UpdateSettings (int userId, SettingsDto settingsDto, string langaugeCode);
        Task<MemberDto> GetMemberAsync(int id);
        
        Task StatisticsIncrementOpenedChests(int userId);
        Task StatisticsIncrementDefeatedEnemies(int userId);
        Task StatisticsIncrementGamesPlayed(int userId);
        Task StatisticsIncrementGamesWon(int userId);
    }
}