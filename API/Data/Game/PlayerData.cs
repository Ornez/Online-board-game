using API.Modules.Game.Utils;
using API.Modules.Game.Keys;

namespace API.Data.Game
{
    public class PlayerData
    {
        public UserData UserData { get; set; }
        public ChampionType ChampionType { get; set; }
        public Position Position { get; set; } = new Position(AllKeys.MapKeys.MAP_CENTER, AllKeys.MapKeys.MAP_CENTER);
        public int MovementPoints { get; set; } = 4;
        public int HealthPoints { get; set; } = 6;
        public float Treasures { get; set; } = 0;
        public EquipmentData Equipment { get; set; } = new();
        public bool IsCursed { get; set; } = false;
        public bool IsConnected { get; set; } = false;
        public int QueueOrder { get; set; }
        public int RollsResult { get; set; }

        public PlayerData()
        {
        }
        
        public PlayerData(UserData userData, ChampionType championType) 
        {
            UserData = userData;
            ChampionType = championType;
        }
    }
}

