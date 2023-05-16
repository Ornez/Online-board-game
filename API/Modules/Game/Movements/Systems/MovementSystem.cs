using API.Data.Game;
using API.Hubs;
using API.Modules.Game.Managers;
using API.Modules.Game.Enemies;
using API.Modules.Game.Fights.Responses;
using API.Modules.Game.Keys;
using API.Modules.Game.Movements.Responses;
using API.Modules.Game.Pahtfindings;
using API.Modules.Game.Utils;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game.Movements.Systems;

public class MovementSystem : BaseSystem, IMovementSystem
{
    private readonly IPathfinding _pathfinding;

    public MovementSystem(IGameManager gameManager, IHubContext<GameHub> gameHubContext,
        IPathfinding pathfinding) : base(gameManager, gameHubContext)
    {
        _pathfinding = pathfinding;
    }

    public async Task<PlayerMovedResponse> Move(UserData userData, Position position)
    {
        var gameData = CurrentGame(userData);
        var playerData = CurrentPlayer(userData);

        var start = gameData.Map.RouteFields[playerData.Position.X, playerData.Position.Y];
        var end = gameData.Map.RouteFields[position.X, position.Y];
        var path = _pathfinding.GetPath(start, end, gameData.Map);
        
        Enemy? enemy = gameData.Map.RouteFields[position.X, position.Y].Enemy;
        
        playerData.Position = position;
        playerData.MovementPoints -= path.Count;
        if (enemy != null)
            playerData.MovementPoints = 0;

        if (playerData.MovementPoints < 0)
            playerData.MovementPoints = 0;
        
        PlayerMovedResponse playerMovedResponse = new(path, userData, playerData.MovementPoints);
        await CurrentGroupExceptCurrentUser(userData).SendAsync(AllKeys.GameKeys.PLAYER_MOVED, playerMovedResponse);
        
        if (enemy != null)
        {
            var fightStartedResponse = new FightStartedResponse(enemy.EnemyType.ToString(), userData, enemy.Strength);
            await CurrentGroup(userData).SendAsync(AllKeys.GameKeys.FIGHT_STARTED, fightStartedResponse);
        }

        gameData.Map.SelectedRouteField = null;
        return playerMovedResponse;
    }

    public List<Position> GetPositionsToMove(UserData userData)
    {
        var gameData = CurrentGame(userData);
        var map = gameData.Map;
        var playerData = CurrentPlayer(userData);
        List<Position> positionsToMove = new();
        
        for (var y = 0; y < AllKeys.MapKeys.MAP_SIZE; y++)
        {
            for (var x = 0; x < AllKeys.MapKeys.MAP_SIZE; x++)
            {
                if (x == playerData.Position.X && y == playerData.Position.Y)
                    continue;
                
                if (map.RouteFields[x,y] != null
                    && _pathfinding.CalculatePath(
                        map.RouteFields[playerData.Position.X, playerData.Position.Y],
                        map.RouteFields[x, y], map).Count <= playerData.MovementPoints)
                    positionsToMove.Add(new Position(x, y));
            }
        }
        return positionsToMove;
    }

    public async Task TeleportToPosition(UserData userData, Position position)
    {
        var player = CurrentPlayer(userData);
        player.Position = position;
        player.MovementPoints = 0;
    }
}