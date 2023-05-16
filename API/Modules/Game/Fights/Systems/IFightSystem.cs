using API.Data.Game;
using API.Modules.Game.Fights.Responses;

namespace API.Modules.Game.Fights.Systems;

public interface IFightSystem
{
    Task<List<int>> RollDice(UserData userData);
    Task<FightStatusResponse> CalculateFight(UserData userData, int damageScrollsUsed);
    Task<FightOverResponse> SettleTheFight(UserData userData, int damageScrollsUsed);
}