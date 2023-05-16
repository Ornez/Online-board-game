using API.Data.Game;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Managers
{
    public interface IGameManager
    {
        void AddPlayer(UserData userData, int gameId);
        void RegisterNewConnection(UserData userData, string connectionId);
        void UnregisterConnection(UserData userData);
        
        List<PlayerData> GetPlayers(int gameId);
        int GetGameId(UserData userData);

        void SetUserAsConnected(UserData userData);
        void SetUserAsDisconnected(UserData userData);
        bool AreAllUsersConnected(UserData userData);

        Task StartNextRound(UserData userData);

        
        PlayerData GetPlayerDataByUser(UserData userData);
        string GetConnectionIdByUser(UserData userData);
        GameData GetGameDataByUser(UserData userData);
    }
}