using API.Extensions;
using API.Modules.Game.Keys;
using API.Modules.Game.Maps;

namespace API.Modules.Game.Enemies.Systems;

public class EnemySystem
{
    public const int RATS_COUNT = 8;
    public const int SPIDERs_COUNT = 4;
    public const int MUMMIES_COUNT = 8;
    public const int KEY_KEEPERS_COUNT = 12;
    public const int SKELETONS_COUNT = 5;
    public const int GHOSTS_COUNT = 3;
    public const int VAMPIRES_COUNT = 2;
    public const int DRAGONS_COUNT = 1;
    
    public static void AssignEnemiesToRouteFields(Map map)
    {
        List<RouteField> rooms = map.AvailableRouteFields.FindAll(routeField => routeField.IsRoom);
        
        List<Enemy> availableEnemies = GenerateAvailableEnemies();
        availableEnemies.Shuffle();
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i < availableEnemies.Count)
                rooms[i].Enemy = availableEnemies[i];
            else
                rooms[i].IsChest = true;
        }
    }

    private static List<Enemy> GenerateAvailableEnemies()
    {
        List<Enemy> availableEnemies = new List<Enemy>();
        for (int i = 0; i < RATS_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.RAT));
        for (int i = 0; i < SPIDERs_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.SPIDER));
        for (int i = 0; i < MUMMIES_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.MUMMY));
        for (int i = 0; i < KEY_KEEPERS_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.KEY_KEEPER));
        for (int i = 0; i < SKELETONS_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.SKELETON));
        for (int i = 0; i < GHOSTS_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.GHOST));
        for (int i = 0; i < VAMPIRES_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.VAMPIRE));
        for (int i = 0; i < DRAGONS_COUNT; i++)
            availableEnemies.Add(EnemyBuilder.BuildEnemy(EnemyType.DRAGON));
        return availableEnemies;
    }
}