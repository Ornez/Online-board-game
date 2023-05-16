using API.Modules.Game.Utils;

namespace API.Data.Game.OutputMessages;

public class PlayerSelectedOutput
{
    public UserData UserData { get; set; }
    public string CharacterName { get; set; }
    public Position Position { get; set; }
    public int MovementPoints { get; set; }
    public int HealthPoints { get; set; }
    public float Treasures { get; set; }
    public EquipmentData Equipment { get; set; }
    public int QueueOrder { get; set; }

    public PlayerSelectedOutput(PlayerData playerData)
    {
        UserData = playerData.UserData;
        CharacterName = playerData.ChampionType.ToString();
        Position = playerData.Position;
        MovementPoints = playerData.MovementPoints;
        HealthPoints = playerData.HealthPoints;
        Treasures = playerData.Treasures;
        Equipment = playerData.Equipment;
        QueueOrder = playerData.QueueOrder;
    }
}