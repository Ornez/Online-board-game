using API.Data.Game;
using API.Hubs;
using API.Modules.Game.Managers;
using API.Modules.Game.Enemies;
using API.Modules.Game.Enemies.Responses;
using API.Modules.Game.Keys;
using API.Modules.Game.Maps.Responses;
using API.Modules.Game.Pahtfindings;
using API.Modules.Game.Utils;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game.Maps.Systems;

public class MapSystem : BaseSystem, IMapSystem
{
    private readonly IPathfinding _pathfinding;
    private readonly Random _random = new();

    public MapSystem(IGameManager gameManager, IHubContext<GameHub> gameHubContext, IPathfinding pathfinding) 
        : base(gameManager, gameHubContext)
    {
        _pathfinding = pathfinding;
    }

    public async Task PrepareMap(UserData userData)
    {
        Map map = CurrentGame(userData).Map;
        RouteField startRounteField = new RouteField(new[] { true, true, true, true }, 0, true, true);
        map.SelectedRouteField = startRounteField;
        await PlaceRouteField(userData, new Position(10, 10), 0);
        map.SelectedRouteField = null;
    }
    
    public static void GenerateAvailableRouteFields(Map map)
    {
        // 75 RouteFields + 1 StartRouteField = 76 RouteFields
        // 20 Corridors
        for (var i = 0; i < 4; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { true, true, false, false }, 0, false, false));
        for (var i = 0; i < 4; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { true, false, true, false }, 0, false, false));
        for (var i = 0; i < 5; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { false, true, true, true }, 0, false, false));
        for (var i = 0; i < 7; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { true, true, true, true }, 0, false, false));

        //  2 Fountains
        // for (var i = 0; i < 2; i++)
        //     map.AvailableRouteFields.Add(new RouteField(new[] { true, true, false, false }, 0, false, true));

        // 53 Rooms
        for (var i = 0; i < 13; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { true, true, false, false }, 0, true, false));
        for (var i = 0; i < 13; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { true, false, true, false }, 0, true, false));
        for (var i = 0; i < 13; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { false, true, true, true }, 0, true, false));
        for (var i = 0; i < 14; i++)
            map.AvailableRouteFields.Add(new RouteField(new[] { true, true, true, true }, 0, true, false));
    }

    public RouteField GetRandomRouteField(Map map)
    {

        
        var randomIndex = _random.Next(map.AvailableRouteFields.Count);
        var routeField = map.AvailableRouteFields[randomIndex];
        map.AvailableRouteFields.RemoveAt(randomIndex);
        map.SelectedRouteField = routeField;
        Console.WriteLine("ROUTE FIELD TAKEN");
        return routeField;
    }

    public RouteFieldPlacementLocationsResponse GetRouteFieldPlacementLocations(Map map, RouteField routeField,
        PlayerData playerData)
    {
        RouteFieldPlacementLocationsResponse routeFieldPlacementLocationsResponse = new("", new());
        routeFieldPlacementLocationsResponse.PositionsToPlace = new List<List<Position>>();
        routeFieldPlacementLocationsResponse.FieldType = GetRouteFieldType(routeField).ToString();

        var tempRouteFieldRotation = routeField.Rotation;

        for (var rotation = 0; rotation < 4; rotation++)
        {
            routeField.Rotation = rotation;
            routeFieldPlacementLocationsResponse.PositionsToPlace.Add(new List<Position>());

            for (var y = 0; y < AllKeys.MapKeys.MAP_SIZE; y++)
            for (var x = 0; x < AllKeys.MapKeys.MAP_SIZE; x++)
            {
                if (map.RouteFields[x,y] != null)
                    continue;
                
                if (y + 1 < AllKeys.MapKeys.MAP_SIZE
                    && map.RouteFields[x, y + 1] != null
                    && routeField.TopConnection && map.RouteFields[x, y + 1].BottomConnection
                    && _pathfinding.CalculateMovementPointsCost(
                        map.RouteFields[playerData.Position.X, playerData.Position.Y],
                        new Position(x, y), map) <= playerData.MovementPoints)
                    routeFieldPlacementLocationsResponse.PositionsToPlace[rotation].Add(new Position(x, y));
                else if (x + 1 < AllKeys.MapKeys.MAP_SIZE
                         && map.RouteFields[x + 1, y] != null
                         && routeField.RightConnection && map.RouteFields[x + 1, y].LeftConnection
                         && _pathfinding.CalculateMovementPointsCost(
                             map.RouteFields[playerData.Position.X, playerData.Position.Y],
                             new Position(x, y), map) <= playerData.MovementPoints)
                    routeFieldPlacementLocationsResponse.PositionsToPlace[rotation].Add(new Position(x, y));
                else if (y - 1 >= 0
                         && map.RouteFields[x, y - 1] != null
                         && routeField.BottomConnection && map.RouteFields[x, y - 1].TopConnection
                         && _pathfinding.CalculateMovementPointsCost(
                             map.RouteFields[playerData.Position.X, playerData.Position.Y],
                             new Position(x, y), map) <= playerData.MovementPoints)
                    routeFieldPlacementLocationsResponse.PositionsToPlace[rotation].Add(new Position(x, y));
                else if (x - 1 >= 0
                         && map.RouteFields[x - 1, y] != null
                         && routeField.LeftConnection && map.RouteFields[x - 1, y].RightConnection
                         && _pathfinding.CalculateMovementPointsCost(
                             map.RouteFields[playerData.Position.X, playerData.Position.Y],
                             new Position(x, y), map) <= playerData.MovementPoints)
                    routeFieldPlacementLocationsResponse.PositionsToPlace[rotation].Add(new Position(x, y));
            }
        }

        routeField.Rotation = tempRouteFieldRotation;
        return routeFieldPlacementLocationsResponse;
    }

    public RouteFieldType GetRouteFieldType(RouteField routeField)
    {
        if (routeField.IsFountain && routeField.IsRoom) 
            return RouteFieldType.START;

        if (routeField.IsFountain)
        {
            if (routeField.Connections[0] && routeField.Connections[1] && routeField.Connections[2]
                && routeField.Connections[3])
                return RouteFieldType.HEALING_X;
            if (routeField.Connections[1] && routeField.Connections[2] && routeField.Connections[3])
                return RouteFieldType.HEALING_T;
            if (routeField.Connections[0] && routeField.Connections[2])
                return RouteFieldType.HEALING_I;
            return RouteFieldType.HEALING_L;
        }

        if (routeField.IsRoom)
        {
            if (routeField.Connections[0] && routeField.Connections[1] && routeField.Connections[2]
                && routeField.Connections[3])
                return RouteFieldType.CHAMBER_X;
            if (routeField.Connections[1] && routeField.Connections[2] && routeField.Connections[3])
                return RouteFieldType.CHAMBER_T;
            if (routeField.Connections[0] && routeField.Connections[2])
                return RouteFieldType.CHAMBER_I;
            return RouteFieldType.CHAMBER_L;
        }

        if (routeField.Connections[0] && routeField.Connections[1] && routeField.Connections[2]
            && routeField.Connections[3])
            return RouteFieldType.CORRIDOR_X;
        if (routeField.Connections[1] && routeField.Connections[2] && routeField.Connections[3])
            return RouteFieldType.CORRIDOR_T;
        if (routeField.Connections[0] && routeField.Connections[2])
            return RouteFieldType.CORRIDOR_I;
        return RouteFieldType.CORRIDOR_L;
    }
    
    public async Task PlaceRouteField(UserData userData, Position position, int rotation)
    {
        Map map = CurrentGame(userData).Map;
        RouteField routeField = map.SelectedRouteField;
        routeField.Rotation = rotation;
        routeField.Position = position;
        
        map.RouteFields[position.X, position.Y] = routeField;

        Enemy? enemy = routeField.Enemy;
        EnemyResponse? enemyResponse = null;

        if (routeField.IsRoom)
            CurrentPlayer(userData).MovementPoints = 0;
        
        if (enemy != null)
            enemyResponse = new(enemy.EnemyType.ToString(), enemy.ItemReward?.Name ?? "CHEST", enemy.Strength);
        
        RouteFieldPlacedResponse routeFieldPlacedResponse = new(GetRouteFieldType(routeField).ToString(),
            position, routeField.Rotation * 90, enemyResponse, routeField.IsChest);

        await CurrentGroup(userData).SendAsync(AllKeys.MapKeys.ROUTE_FIELD_PLACED, routeFieldPlacedResponse);

        if (enemy != null)
        {
            await CurrentGroup(userData).SendAsync(AllKeys.GameKeys.ENEMY_SPAWNED, enemyResponse);
        }
    }

    public RouteField GetClosestFountain(RouteField routeField, Map map)
    {
        List<RouteField> fountains = GetAllFountains(map);
        
        int minDistance = int.MaxValue;
        RouteField? closestFountain = null;        
        foreach (RouteField fountain in fountains)
        {
            int distance = _pathfinding.CalculateMovementPointsCost(routeField, fountain.Position, map);
            if (minDistance > distance)
            {
                minDistance = distance;
                closestFountain = fountain;
            }
        }

        return closestFountain;
    }

    private List<RouteField> GetAllFountains(Map map)
    {
        List<RouteField> fountains = new();
        for (int y = 0; y < AllKeys.MapKeys.MAP_SIZE; y++)
        {
            for (int x = 0; x < AllKeys.MapKeys.MAP_SIZE; x++)
            {
                if (map.RouteFields[x,y] != null && map.RouteFields[x, y].IsFountain)
                    fountains.Add(map.RouteFields[x, y]);
            }
        }

        return fountains;
    }
}