using API.Data.Game;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Movements.Responses;

public class PlayerMovedResponse
{
    public PlayerMovedResponse(List<Position> path, UserData userData, int movementPoints)
    {
        Path = path;
        UserData = userData;
        MovementPoints = movementPoints;
    }

    public List<Position> Path { get; set; }
    public UserData UserData { get; set; }
    public int MovementPoints { get; set; }
}