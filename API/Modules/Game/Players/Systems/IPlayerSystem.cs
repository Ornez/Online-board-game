using API.Data.Game;

namespace API.Modules.Game.Players.Systems;

public interface IPlayerSystem
{ 
    Task Heal(UserData userData);
}