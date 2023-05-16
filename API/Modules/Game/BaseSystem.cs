using API.Data.Game;
using API.Hubs;
using API.Modules.Game.Managers;
using API.Modules.Game.Maps;
using API.Modules.Game.Utils;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game;

public abstract class BaseSystem
{
    private readonly IGameManager _gameManager;
    private readonly IHubContext<GameHub> _gameHubContext;

    protected BaseSystem(IGameManager gameManager, IHubContext<GameHub> gameHubContext)
    {
        _gameManager = gameManager;
        _gameHubContext = gameHubContext;
    }

    protected PlayerData CurrentPlayer(UserData userData)
    {
        return _gameManager.GetPlayerDataByUser(userData);
    }

    protected string CurrentConnectionId(UserData userData)
    {
        return _gameManager.GetConnectionIdByUser(userData);
    }
    
    protected GameData CurrentGame(UserData userData)
    {
        return _gameManager.GetGameDataByUser(userData);
    }
    
    protected RouteField CurrentField(UserData userData)
    {
        Position playerPosition = CurrentPlayer(userData).Position;
        return CurrentGame(userData).Map.RouteFields[playerPosition.X, playerPosition.Y];
    }

    protected IHubContext<GameHub> CurrentGameHubContext()
    {
        return _gameHubContext;
    }

    protected IClientProxy CurrentGroupExceptCurrentUser(UserData userData)
    {
        int gameId = CurrentGame(userData).GameId;
        string connectionId = CurrentConnectionId(userData);
        return _gameHubContext.Clients.GroupExcept(gameId.ToString(), connectionId);
    }
    
    protected IClientProxy CurrentGroup(UserData userData)
    {
        int gameId = CurrentGame(userData).GameId;
        return _gameHubContext.Clients.Group(gameId.ToString());
    }
}