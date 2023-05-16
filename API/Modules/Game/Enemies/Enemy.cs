using API.Modules.Game.Inventories;

namespace API.Modules.Game.Enemies;

public class Enemy
{
    public Enemy(EnemyType enemyType, int strength, Item itemReward, float pointsReward = 0)
    {
        EnemyType = enemyType;
        Strength = strength;
        ItemReward = itemReward;
        PointsReward = pointsReward;
    }

    public EnemyType EnemyType { get; set; }
    public int Strength { get; set; }
    public Item? ItemReward { get; set; }
    public float PointsReward { get; set; }
}