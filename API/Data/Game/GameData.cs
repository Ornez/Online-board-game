using API.Modules.Game.Maps;

namespace API.Data.Game
{
    public class GameData
    {
        public int GameId { get; set; }
        public List<PlayerData> Players { get; set; } = new();
        public Map Map { get; set; } = new();
        public int RoundNumber { get; set; } = -1;
        public UserData CurrentUser { get; set; }
        
        public GameData(int gameId)
        {
            GameId = gameId;
        }
    }
}