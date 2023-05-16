using API.Data.Game;
using API.Extensions;
using API.Modules.Game.Enemies.Systems;
using API.Modules.Game.Keys;
using API.Modules.Game.Maps.Systems;
using API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game.Managers;

public class GameManager : IGameManager
{
    private readonly Dictionary<string, string> _connectionIds = new();
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly List<GameData> _games = new();

    public GameManager(IHubContext<GameHub> gameHubContext)
    {
        _gameHubContext = gameHubContext;
    }

    public void AddPlayer(UserData userData, int gameId)
    {
        var gameData = _games.Find(x => x.GameId == gameId) ?? CreateGame(gameId);
        PlayerData playerData = new(userData, PickUniqueRandomChampion(gameData));
        playerData.QueueOrder = gameData.Players.Count;
        gameData.Players.Add(playerData);
    }

    public void RemovePlayer(UserData userData, int gameId)
    {
        var gameData = _games.Find(x => x.GameId == gameId);
        PlayerData playerData = gameData.Players.Find(player => player.UserData.IsSameUserAs(userData));
        gameData.Players.Remove(playerData);
        if (gameData.Players.Count == 0)
        {
            RemoveGame(gameData);
        }
    }

    private void RemoveGame(GameData game)
    {
        _games.Remove(game);
    }

    public void RegisterNewConnection(UserData userData, string connectionId)
    {
        _connectionIds.Add(GetConnectionIdKey(userData), connectionId);
    }
    
    public void UnregisterConnection(UserData userData)
    {
        _connectionIds.Remove(GetConnectionIdKey(userData));
    }

    public List<PlayerData> GetPlayers(int gameId)
    {
        var gameData = _games.Find(x => x.GameId == gameId)
                       ?? throw new ArgumentException($"Game with id: {gameId} doesn't exists.");
        return gameData.Players;
    }

    public int GetGameId(UserData userData)
    {
        return GetGameDataByUser(userData).GameId;
    }

    public GameData GetGameDataByUser(UserData userData)
    {
        var gameData = _games.Find(x => x.Players.Find(y => y.UserData.IsSameUserAs(userData)) != null)
                       ?? throw new ArgumentException($"Game with user: {userData} doesn't exists.");
        return gameData;
    }

    public void SetUserAsConnected(UserData userData)
    {
        GetPlayerDataByUser(userData).IsConnected = true;
    }

    public void SetUserAsDisconnected(UserData userData)
    {
        GetPlayerDataByUser(userData).IsConnected = false;
    }
    
    public PlayerData GetPlayerDataByUser(UserData userData)
    {
        return GetGameDataByUser(userData).Players.Find(player => player.UserData.IsSameUserAs(userData));
    }

    public bool AreAllUsersConnected(UserData userData)
    {
        var notConnectedPlayerData = GetGameDataByUser(userData).Players
            .Find(player => player.IsConnected == false);
        return notConnectedPlayerData == null;
    }

    public async Task StartNextRound(UserData userData)
    {
        var gameData = GetGameDataByUser(userData);
        gameData.RoundNumber++;

        var currentPlayerTurn = gameData.Players
            .Find(player => player.QueueOrder == gameData.RoundNumber % gameData.Players.Count);
        gameData.CurrentUser = currentPlayerTurn.UserData;

        GetPlayerDataByUser(currentPlayerTurn.UserData).MovementPoints = 4;
        
        await _gameHubContext.Clients.Group(gameData.GameId.ToString())
            .SendAsync(AllKeys.GameKeys.ROUND_STARTED, currentPlayerTurn.UserData);
    }

    public string GetConnectionIdByUser(UserData userData)
    {
        return _connectionIds[GetConnectionIdKey(userData)];
    }

    private string GetConnectionIdKey(UserData userData)
    {
        return $"{userData.Role}_{userData.Id}";
    }

    private GameData CreateGame(int gameId)
    {
        GameData gameData = new(gameId);
        MapSystem.GenerateAvailableRouteFields(gameData.Map);
        EnemySystem.AssignEnemiesToRouteFields(gameData.Map);
        _games.Add(gameData);
        return gameData;
    }

    private ChampionType PickUniqueRandomChampion(GameData gameData)
    {
        var championTypes = Enum.GetValues(typeof(ChampionType));
        var random = new Random();
        ChampionType championType;

        var iterations = 0;
        do
        {
            championType = (ChampionType)championTypes.GetValue(random.Next(championTypes.Length));
            iterations++;
            if (iterations >= 100)
                throw new Exception("There is not enough champion types for that amount of players in game.");
        } while (gameData.Players.FirstOrDefault(x => x.ChampionType == championType) != null);

        return championType;
    }
}