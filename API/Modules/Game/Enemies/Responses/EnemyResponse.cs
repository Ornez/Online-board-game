namespace API.Modules.Game.Enemies.Responses;

public class EnemyResponse
{
    public EnemyResponse(string name, string itemReward, int strength)
    {
        Name = name;
        ItemReward = itemReward;
        Strength = strength;
    }

    public string Name { get; set; }
    public string ItemReward { get; set; }
    public int Strength { get; set; }
}