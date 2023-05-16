namespace API.Data.Game
{
    public class PlayerDataToDisplay
    {
        public UserData UserData { get; set; }
        public string CharacterName { get; set; }
        public int HealthPoints { get; set; } = 5;
        public float Treasures { get; set; } = 0;
        public EquipmentData Equipment { get; set; } = new();
        public bool IsCursed { get; set; } = false;
    }
}