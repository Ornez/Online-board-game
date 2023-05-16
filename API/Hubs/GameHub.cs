using API.Data;
using API.Data.Game;
using API.Data.Game.OutputMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using API.Modules.Game.Managers;
using API.Extensions;
using API.Modules.Game.EndGame.Systems;
using API.Modules.Game.Enemies;
using API.Modules.Game.Fights.Requests;
using API.Modules.Game.Fights.Responses;
using API.Modules.Game.Fights.Systems;
using API.Modules.Game.Inventories.Systems;
using API.Modules.Game.Maps;
using API.Modules.Game.Maps.Systems;
using API.Modules.Game.Utils;
using API.Modules.Game.Keys;
using API.Modules.Game.Maps.Requests;
using API.Modules.Game.Maps.Responses;
using API.Modules.Game.Movements.Responses;
using API.Modules.Game.Movements.Systems;
using API.Modules.Game.Players.Responses;
using API.Modules.Game.Players.Systems;
using API.Interfaces;

namespace API.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly IGameManager _gameManager;
    private readonly IMapSystem _mapSystem;
    private readonly IMovementSystem _movementSystem;
    private readonly IFightSystem _fightSystem;
    private readonly IInventorySystem _inventorySystem;
    private readonly IPlayerSystem _playerSystem;
    private readonly IEndGameSystem _endGameSystem;
    private readonly IServiceProvider _serviceProvider;
    
    public GameHub(IGameManager gameManager, IMapSystem mapSystem, IMovementSystem movementSystem,
        IFightSystem fightSystem, IInventorySystem inventorySystem, IPlayerSystem playerSystem,
        IEndGameSystem endGameSystem, IServiceProvider serviceProvider)
    {
        _gameManager = gameManager;
        _mapSystem = mapSystem;
        _movementSystem = movementSystem;
        _fightSystem = fightSystem;
        _inventorySystem = inventorySystem;
        _playerSystem = playerSystem;
        _endGameSystem = endGameSystem;
        _serviceProvider = serviceProvider;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        _gameManager.RegisterNewConnection(Context.User!.UserData(), Context.ConnectionId);
        
        int gameId = _gameManager.GetGameId(Context.User!.UserData());
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
        
        _gameManager.SetUserAsConnected(Context.User!.UserData());
        if (_gameManager.AreAllUsersConnected(Context.User!.UserData()))
        {
            await _mapSystem.PrepareMap(Context.User!.UserData());
            await SendPlayersSelectedOutput();
            await _gameManager.StartNextRound(Context.User!.UserData());
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        int gameId = _gameManager.GetGameId(Context.User!.UserData());
        
        _gameManager.SetUserAsDisconnected(Context.User!.UserData());
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
        
        _gameManager.UnregisterConnection(Context.User!.UserData());
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendPlayersSelectedOutput()
    {
        int gameId = _gameManager.GetGameId(Context.User.UserData());
        List<PlayerSelectedOutput> playersSelectedOutputs = new();
        foreach (PlayerData playerData in _gameManager.GetPlayers(gameId))
        {
            PlayerSelectedOutput playerSelectedOutput = new(playerData);
            playersSelectedOutputs.Add(playerSelectedOutput);
        }
        await Clients.Group(gameId.ToString()).SendAsync(AllKeys.GameKeys.PLAYERS_SELECTED, playersSelectedOutputs);
    }
    
    public async Task<RouteFieldPlacementLocationsResponse> GetRouteField()
    {
        GameData gameData = _gameManager.GetGameDataByUser(Context.User!.UserData());
        RouteField routeField = _mapSystem.GetRandomRouteField(gameData.Map);
        if (routeField == null)
            return new("", new());

        SelectedRouteFieldResponse selectedRouteFieldResponse = new(_mapSystem.GetRouteFieldType(routeField));
        await Clients.GroupExcept(gameData.GameId.ToString(), Context.ConnectionId)
            .SendAsync(AllKeys.GameKeys.ROUTE_FIELD_SELECTED, selectedRouteFieldResponse);
        
        PlayerData playerData = _gameManager.GetPlayerDataByUser(Context.User!.UserData());
        return _mapSystem.GetRouteFieldPlacementLocations(gameData.Map, routeField, playerData);
    }

    public async Task<PlayerMovedResponse> SetRouteField(SetRouteFieldRequest request)
    {
        await _mapSystem.PlaceRouteField(Context.User!.UserData(), request.Position, request.Rotation / 90);
        return await _movementSystem.Move(Context.User!.UserData(), request.Position);
    }

    public async Task<PlayerMovedResponse> Move(Position position)
    {
        return await _movementSystem.Move(Context.User!.UserData(), position);
    }

    public async Task<List<Position>> GetPositionsToMove()
    {
        return _movementSystem.GetPositionsToMove(Context.User!.UserData());
    }

    public async Task PickUpItem()
    {
        await _inventorySystem.PickUpItem(Context.User!.UserData());
    }
    
    public async Task<List<int>> RollTheDice()
    {
        return await _fightSystem.RollDice(Context.User!.UserData());
    }

    public async Task UseHealingScroll()
    {
        GameData gameData = _gameManager.GetGameDataByUser(Context.User!.UserData());
        PlayerData player = _gameManager.GetPlayerDataByUser(Context.User!.UserData());
        RouteField currentRouteField = gameData.Map.RouteFields[player.Position.X, player.Position.Y];
        RouteField closestFountain = _mapSystem.GetClosestFountain(currentRouteField, gameData.Map);
        await _movementSystem.TeleportToPosition(Context.User!.UserData(), closestFountain.Position);
        await _playerSystem.Heal(Context.User!.UserData());
        await _inventorySystem.RemoveHealingScroll(Context.User!.UserData());
        
        PlayerUpdatedResponse playerUpdatedResponse = new(Context.User!.UserData(), player.Position, player.MovementPoints,
            player.HealthPoints, player.Treasures, player.Equipment);
        await Clients.Group(gameData.GameId.ToString()).SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);
    }
    
    public async Task<FightStatusResponse> CalculateFight(CalculateFightRequest request)
    {
        return await _fightSystem.CalculateFight(Context.User!.UserData(), request.DamageScrollsUsed);
    }
    
    public async Task<FightOverResponse> StartFight(CalculateFightRequest request)
    {
        GameData gameData = _gameManager.GetGameDataByUser(Context.User!.UserData());
        PlayerData currentPlayer = _gameManager.GetPlayerDataByUser(Context.User!.UserData());
        RouteField currentRouteField = gameData.Map.RouteFields[currentPlayer.Position.X, currentPlayer.Position.Y];
        Enemy enemy = currentRouteField.Enemy;
        FightOverResponse fightOverResponse = await _fightSystem.SettleTheFight(Context.User!.UserData(), request.DamageScrollsUsed);
        if (enemy.EnemyType == EnemyType.DRAGON && fightOverResponse.FightResult == AllKeys.GameKeys.FIGHT_VICTORY)
        {
            var endGameResponse = _endGameSystem.GetEndGameResponse(Context.User!.UserData());

            foreach (var endGameUserResponse in endGameResponse.FirstPlace)
            {
                if (endGameUserResponse.UserData.Role != AllKeys.Roles.USER)
                    continue;
                
                using var scope = _serviceProvider.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                await userRepository.StatisticsIncrementGamesWon(endGameUserResponse.UserData.Id);
            }

            foreach (var player in gameData.Players)
            {
                if (player.UserData.Role != AllKeys.Roles.USER)
                    continue;
                
                using var scope = _serviceProvider.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                await userRepository.StatisticsIncrementGamesPlayed(player.UserData.Id);
            }
            
            await Clients.Group(gameData.GameId.ToString()).SendAsync(AllKeys.GameKeys.GAME_ENDED, endGameResponse);
        }
        return fightOverResponse;
    }

    public async Task<ReturnMessage> OpenChest()
    {
        return await _inventorySystem.OpenChest(Context.User!.UserData());
    }
    
    public async Task RoundOver()
    {
        GameData gameData = _gameManager.GetGameDataByUser(Context.User!.UserData());
        Map map = gameData.Map;
        if (map.SelectedRouteField != null)
        {
            Console.WriteLine("ROUTE FIELD RETURNED");
            map.AvailableRouteFields.Add(map.SelectedRouteField);
            map.SelectedRouteField = null;
        }
        
        await _gameManager.StartNextRound(Context.User!.UserData());
    }
}
