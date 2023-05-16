using API.Data;
using API.Data.Game;
using API.DTOs;
using API.Hubs;
using API.Interfaces;
using API.Modules.Game.Managers;
using API.Modules.Game.Enemies;
using API.Modules.Game.Enemies.Responses;
using API.Modules.Game.Keys;
using API.Modules.Game.Maps;
using API.Modules.Game.Maps.Responses;
using API.Modules.Game.Players.Responses;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game.Inventories.Systems;

public class InventorySystem : BaseSystem, IInventorySystem
{
    private readonly IServiceProvider _serviceProvider;
    
    public InventorySystem(IGameManager gameManager, IHubContext<GameHub> gameHubContext, IServiceProvider serviceProvider) 
        : base(gameManager, gameHubContext)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task PickUpItem(UserData userData)
    {
        RouteField currentRouteField = CurrentField(userData);
        Item currentItem = currentRouteField.Item;
        if (currentItem == null)
            return;

        PlayerData player = CurrentPlayer(userData);
        if (currentItem.ItemType == ItemType.Key)
        {
            if (player.Equipment.Key == false)
            {
                player.Equipment.Key = true;
                currentRouteField.Item = null;
            }
        }
        else if (currentItem.ItemType == ItemType.Weapon)
        {
            if (player.Equipment.Weapon1 == null)
            {
                player.Equipment.Weapon1 = currentItem as Weapon;
                currentRouteField.Item = null;
            }
            else if (player.Equipment.Weapon2 == null)
            {
                player.Equipment.Weapon2 = currentItem as Weapon;
                currentRouteField.Item = null;
            }
            else
            {
                Weapon weaponToPickUp = currentItem as Weapon;
                if (player.Equipment.Weapon1.Damage < weaponToPickUp.Damage)
                {
                    currentRouteField.Item = player.Equipment.Weapon1;
                    player.Equipment.Weapon1 = weaponToPickUp;
                }
                else if (player.Equipment.Weapon2.Damage < weaponToPickUp.Damage)
                {
                    currentRouteField.Item = player.Equipment.Weapon2;
                    player.Equipment.Weapon2 = weaponToPickUp;
                }
            }
        }
        else if (currentItem.ItemType == ItemType.DamageScroll || currentItem.ItemType == ItemType.HealingScroll)
        {
            if (player.Equipment.Scroll1 == null)
            {
                player.Equipment.Scroll1 = currentItem as Scroll;
                currentRouteField.Item = null;
            }
            else if (player.Equipment.Scroll2 == null)
            {
                player.Equipment.Scroll2 = currentItem as Scroll;
                currentRouteField.Item = null;
            }
            else if (player.Equipment.Scroll3 == null)
            {
                player.Equipment.Scroll3 = currentItem as Scroll;
                currentRouteField.Item = null;
            }
            else
            {
                Scroll scrollToPickUp = currentItem as Scroll;
                if (player.Equipment.Scroll1.ItemType != currentItem.ItemType)
                {
                    currentRouteField.Item = player.Equipment.Scroll1;
                    player.Equipment.Scroll1 = scrollToPickUp;
                }
                else if (player.Equipment.Scroll2.ItemType != currentItem.ItemType)
                {
                    currentRouteField.Item = player.Equipment.Scroll2;
                    player.Equipment.Scroll2 = scrollToPickUp;
                }
                else if (player.Equipment.Scroll3.ItemType != currentItem.ItemType)
                {
                    currentRouteField.Item = player.Equipment.Scroll3;
                    player.Equipment.Scroll3 = scrollToPickUp;
                }
            }
        }

        Enemy? enemy = currentRouteField.Enemy;
        EnemyResponse? enemyResponse = null;

        if (enemy != null)
            enemyResponse = new(enemy.EnemyType.ToString(), enemy.ItemReward?.Name ?? "CHEST", enemy.Strength);
        
        string itemName = currentRouteField.Item?.Name ?? "";
        
        RouteFieldUpdatedResponse routeFieldUpdatedResponse =
            new(player.Position, itemName, enemyResponse, currentRouteField.IsChest);
        await CurrentGroup(userData)
            .SendAsync(AllKeys.GameKeys.ROUTE_FIELD_UPDATED, routeFieldUpdatedResponse);
            
        PlayerUpdatedResponse playerUpdatedResponse = new(userData, player.Position, player.MovementPoints,
            player.HealthPoints, player.Treasures, player.Equipment);
        await CurrentGroup(userData)
            .SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);
    }

    public async Task<ReturnMessage> OpenChest(UserData userData)
    {
        RouteField field = CurrentField(userData);
        PlayerData player = CurrentPlayer(userData);
        
        if (!player.Equipment.Key || !field.IsChest)
        {
            return new ReturnMessage(AllKeys.GameMessages.OPEN_CHEST_FAILED, false);
        }

        field.IsChest = false;
        player.Equipment.Key = false;
        player.Treasures += 1;
        
        RouteFieldUpdatedResponse routeFieldUpdatedResponse = new(player.Position, "", null, field.IsChest);
        await CurrentGroup(userData).SendAsync(AllKeys.GameKeys.ROUTE_FIELD_UPDATED, routeFieldUpdatedResponse);
        
        PlayerUpdatedResponse playerUpdatedResponse = new(userData, player.Position, player.MovementPoints,
            player.HealthPoints, player.Treasures, player.Equipment);
        await CurrentGroup(userData).SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);

        if (userData.Role == AllKeys.Roles.USER)
        {
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            await userRepository.StatisticsIncrementOpenedChests(userData.Id);
        }
        return new ReturnMessage(AllKeys.GameMessages.OPEN_CHEST_SUCCESS, true);
    }

    public async Task RemoveHealingScroll(UserData userData)
    {
        PlayerData player = CurrentPlayer(userData);
        EquipmentData equipment = player.Equipment;

        if (equipment.Scroll1 != null && equipment.Scroll1.ItemType == ItemType.HealingScroll)
            equipment.Scroll1 = null;
        else if (equipment.Scroll2 != null && equipment.Scroll2.ItemType == ItemType.HealingScroll)
            equipment.Scroll2 = null;
        else if (equipment.Scroll3 != null && equipment.Scroll3.ItemType == ItemType.HealingScroll)
            equipment.Scroll3 = null;
        
        // PlayerUpdatedResponse playerUpdatedResponse = new(userData, player.Position, player.MovementPoints,
        //     player.HealthPoints, player.Treasures, player.Equipment);
        // await CurrentGroup(userData).SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);
    }
}