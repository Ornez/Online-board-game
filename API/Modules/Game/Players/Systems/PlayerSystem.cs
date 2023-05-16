using API.Data.Game;
using API.Modules.Game.Keys;
using API.Modules.Game.Players.Responses;
using API.Hubs;
using API.Modules.Game.Managers;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game.Players.Systems;

public class PlayerSystem : BaseSystem, IPlayerSystem
{
    public PlayerSystem(IGameManager gameManager, IHubContext<GameHub> gameHubContext) : base(gameManager, gameHubContext)
    {
    }

    public async Task Heal(UserData userData)
    {
        PlayerData player = CurrentPlayer(userData);
        player.HealthPoints = 6;
        // PlayerUpdatedResponse playerUpdatedResponse = new(userData, player.Position, player.MovementPoints,
        //     player.HealthPoints, player.Treasures, player.Equipment);
        // await CurrentGroup(userData).SendAsync(AllKeys.GameKeys.PLAYER_UPDATED, playerUpdatedResponse);
    }
}