using API.Modules.Game.EndGame.Systems;
using API.Modules.Game.Fights.Systems;
using API.Modules.Game.Inventories.Systems;
using API.Modules.Game.Maps.Systems;
using API.Modules.Game.Movements.Systems;
using API.Modules.Game.Pahtfindings;
using API.Modules.Game.Players.Systems;
using API.Modules.Chat.Managers;
using API.Modules.Game.Managers;
using API.Modules.Guests.Managers;

namespace API.Extensions;

public static class GameExtensions
{
    public static IServiceCollection AddGameServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IMovementSystem, MovementSystem>();
        services.AddSingleton<IFightSystem, FightSystem>();
        services.AddSingleton<IInventorySystem, InventorySystem>();
        services.AddSingleton<IPlayerSystem, PlayerSystem>();
        services.AddSingleton<IGameManager, GameManager>();
        services.AddSingleton<IGuestsManager, GuestsManager>();
        services.AddSingleton<IChatManager, ChatManager>();
        services.AddSingleton<IMapSystem, MapSystem>();
        services.AddSingleton<IPathfinding, Pathfinding>();
        services.AddSingleton<IEndGameSystem, EndGameSystem>();
        return services;
    }
}