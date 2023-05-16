using API.Data.Game;

namespace API.Modules.Game.Fights.Responses;

public class FightStartedResponse
{
    public FightStartedResponse(string enemyName, UserData userData, int enemyDamage)
    {
        EnemyName = enemyName;
        UserData = userData;
        EnemyDamage = enemyDamage;
    }

    public string EnemyName { get; set; }
    public int EnemyDamage { get; set; }
    public UserData UserData { get; set; }
}