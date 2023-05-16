using API.Data.Game;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Players.Responses;

public class PlayerUpdatedResponse
{
    public PlayerUpdatedResponse(UserData userData, Position position, int movementPoints, int healthPoints, float treasures, EquipmentData equipment)
    {
        UserData = userData;
        Position = position;
        MovementPoints = movementPoints;
        HealthPoints = healthPoints;
        Treasures = treasures;
        Equipment = equipment;
    }

    public UserData UserData { get; set; }
    public Position Position { get; set; }
    public int MovementPoints { get; set; }
    public int HealthPoints { get; set; }
    public float Treasures { get; set; }
    public EquipmentData Equipment { get; set; }
}