using API.Data.Game;
using API.Hubs;
using API.Interfaces;
using API.Modules.Game.Managers;
using API.Modules.Game.Enemies;
using API.Modules.Game.Enemies.Responses;
using API.Modules.Game.Fights.Responses;
using API.Modules.Game.Inventories;
using API.Modules.Game.Keys;
using API.Modules.Game.Maps;
using API.Modules.Game.Maps.Responses;
using API.Modules.Game.Maps.Systems;
using API.Modules.Game.Movements.Systems;
using API.Modules.Game.Players.Responses;
using API.Modules.Game.Utils;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game.Fights.Systems;

public class FightSystem : BaseSystem, IFightSystem
{
    private readonly Random _random = new();
    private readonly IMovementSystem _movementSystem;
    private readonly IMapSystem _mapSystem;
    private readonly IServiceProvider _serviceProvider;
    
    public FightSystem(IGameManager gameManager, IHubContext<GameHub> gameHubContext, IMovementSystem movementSystem,
        IMapSystem mapSystem, IServiceProvider serviceProvider) 
        : base(gameManager, gameHubContext)
    {
        _movementSystem = movementSystem;
        _mapSystem = mapSystem;
        _serviceProvider = serviceProvider;
    }

    public async Task<List<int>> RollDice(UserData userData)
    {
        List<int> diceRollResults = new List<int> { _random.Next(1, 7), _random.Next(1, 7) };
        CurrentPlayer(userData).RollsResult = diceRollResults[0] + diceRollResults[1];
        
        await CurrentGroupExceptCurrentUser(userData)
            .SendAsync(AllKeys.GameKeys.DICE_ROLLED, new RollDiceResponse(diceRollResults));
        return diceRollResults;
    }

    public async Task<FightStatusResponse> CalculateFight(UserData userData, int damageScrollsUsed)
    {
        PlayerData player = CurrentPlayer(userData);
        
        int playerStrength = player.RollsResult + damageScrollsUsed;
        if (player.Equipment.Weapon1 != null)
            playerStrength += player.Equipment.Weapon1.Damage;
        if (player.Equipment.Weapon2 != null)
            playerStrength += player.Equipment.Weapon2.Damage;
        
        int enemyStrength = CurrentField(userData).Enemy.Strength;
        FightStatusResponse fightStatusResponse = new(userData, playerStrength, enemyStrength, damageScrollsUsed);

        await CurrentGroupExceptCurrentUser(userData).SendAsync(AllKeys.GameKeys.FIGHT_CALCULATED, fightStatusResponse);
        return fightStatusResponse;
    }

    public async Task<FightOverResponse> SettleTheFight(UserData userData, int damageScrollsUsed)
    {
        RouteField field = CurrentField(userData);
        PlayerData player = CurrentPlayer(userData);
        int playerStrength = player.RollsResult + damageScrollsUsed;
        if (player.Equipment.Weapon1 != null)
            playerStrength += player.Equipment.Weapon1.Damage;
        if (player.Equipment.Weapon2 != null)
            playerStrength += player.Equipment.Weapon2.Damage;

        for (int i = damageScrollsUsed - 1; i >= 0; i--)
        {
            if (player.Equipment.Scroll1 != null && player.Equipment.Scroll1.ItemType == ItemType.DamageScroll)
                player.Equipment.Scroll1 = null;
            else if (player.Equipment.Scroll2 != null && player.Equipment.Scroll2.ItemType == ItemType.DamageScroll)
                player.Equipment.Scroll2 = null;
            else if (player.Equipment.Scroll3 != null && player.Equipment.Scroll3.ItemType == ItemType.DamageScroll)
                player.Equipment.Scroll3 = null;
        }
        
        int enemyStrength = field.Enemy!.Strength;

        string fightResult = AllKeys.GameKeys.FIGHT_VICTORY;
        if (playerStrength == enemyStrength)
            fightResult = AllKeys.GameKeys.FIGHT_DRAW;
        else if (playerStrength < enemyStrength)
            fightResult = AllKeys.GameKeys.FIGHT_DEFEAT;
        
        FightOverResponse fightOverResponse = new(fightResult);
        await CurrentGroupExceptCurrentUser(userData).SendAsync(AllKeys.GameKeys.FIGHT_OVER, fightOverResponse);

        if (fightResult == AllKeys.GameKeys.FIGHT_VICTORY)
        {
            field.Item = field.Enemy?.ItemReward ?? null;
            player.Treasures += field.Enemy?.PointsReward ?? 0;
            field.Enemy = null;
            
            string itemName = field.Item?.Name ?? "";
            string enemyName = field.Enemy?.EnemyType.ToString() ?? "";

            Enemy? enemy = field.Enemy;
            EnemyResponse? enemyResponse = null;

            if (enemy != null)
                enemyResponse = new(enemy.EnemyType.ToString(), enemy.ItemReward?.Name ?? "CHEST", enemy.Strength);
            
            
            RouteFieldUpdatedResponse routeFieldUpdatedResponse =
                new(player.Position, itemName, enemyResponse, field.IsChest);
            await CurrentGroup(userData)
                .SendAsync(AllKeys.GameKeys.ROUTE_FIELD_UPDATED, routeFieldUpdatedResponse);
            
            PlayerUpdatedResponse playerUpdatedResponse = new(userData, player.Position, player.MovementPoints,
                player.HealthPoints, player.Treasures, player.Equipment);
            await CurrentGroup(userData)
                .SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);
            
            if (userData.Role == AllKeys.Roles.USER)
            {
                using var scope = _serviceProvider.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                await userRepository.StatisticsIncrementDefeatedEnemies(userData.Id);
            }
        }
        else if (fightResult == AllKeys.GameKeys.FIGHT_DRAW)
        {
            player.MovementPoints = 1;
            Position newPosition = _movementSystem.GetPositionsToMove(userData)[0];
            player.MovementPoints = 0;
            player.Position = newPosition;
            
            PlayerUpdatedResponse playerUpdatedResponse = new(userData, player.Position, player.MovementPoints,
                player.HealthPoints, player.Treasures, player.Equipment);
            await CurrentGroup(userData)
                .SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);
        }
        else
        {
            player.MovementPoints = 1;
            player.Position = _movementSystem.GetPositionsToMove(userData)[0];
            player.MovementPoints = 0;
            
            DamagePlayer(player, CurrentGame(userData).Map);
            
            PlayerUpdatedResponse playerUpdatedResponse = new(userData, player.Position, player.MovementPoints,
                player.HealthPoints, player.Treasures, player.Equipment);
            await CurrentGroup(userData)
                .SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);
        }

        player.RollsResult = 0;
        return fightOverResponse;
    }

    private void DamagePlayer(PlayerData player, Map map)
    {
        player.HealthPoints--;

        if (player.HealthPoints <= 0)
        {
            ResurrectPlayerOnFountain(player, map);
        }
    }

    private void ResurrectPlayerOnFountain(PlayerData player, Map map)
    {
        RouteField closestFountain = _mapSystem.GetClosestFountain(map.RouteFields[player.Position.X, player.Position.Y], map);
        player.Position = closestFountain.Position;
        player.HealthPoints = 6;
    }
}