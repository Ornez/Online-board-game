using API.Data.Game;
using API.Modules.Game.Movements.Responses;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Movements.Systems;

public interface IMovementSystem
{
    Task<PlayerMovedResponse> Move(UserData userData, Position position);
    List<Position> GetPositionsToMove(UserData userData);
    Task TeleportToPosition(UserData userData, Position position);
}