using API.Modules.Game.Inventories;

namespace API.Modules.Game.Enemies;

public class EnemyBuilder
{
    public static Enemy BuildEnemy(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.RAT:
                return new Enemy(EnemyType.RAT, 5, new ShortSword());
            case EnemyType.SPIDER:
                return new Enemy(EnemyType.SPIDER, 6, new HealingScroll());
            case EnemyType.MUMMY:
                return new Enemy(EnemyType.MUMMY, 7, new DamageScroll());
            case EnemyType.KEY_KEEPER:
                return new Enemy(EnemyType.KEY_KEEPER, 8, new Key());
            case EnemyType.SKELETON:
                return new Enemy(EnemyType.SKELETON, 9, new Spear());
            case EnemyType.GHOST:
                return new Enemy(EnemyType.GHOST, 10, new BattleAxe());
            case EnemyType.VAMPIRE:
                return new Enemy(EnemyType.VAMPIRE, 12, null, 1);
            case EnemyType.DRAGON:
                return new Enemy(EnemyType.DRAGON, 15, null, 1.5f);
            default:
                throw new ArgumentException($"{enemyType} is not valid EnemyType.");
        }
    }
}