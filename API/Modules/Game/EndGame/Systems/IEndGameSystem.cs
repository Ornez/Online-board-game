using API.Data.Game;
using API.Modules.Game.EndGame.Responses;

namespace API.Modules.Game.EndGame.Systems;

public interface IEndGameSystem
{
    EndGameResponse GetEndGameResponse(UserData userData);
}