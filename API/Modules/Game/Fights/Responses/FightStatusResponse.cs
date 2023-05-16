using API.Data.Game;

namespace API.Modules.Game.Fights.Responses;

public class FightStatusResponse
{
    public FightStatusResponse(UserData userData, int playerStrength, int enemyStrength, int damageScrollsUsed)
    {
        UserData = userData;
        PlayerStrength = playerStrength;
        EnemyStrength = enemyStrength;
        DamageScrollsUsed = damageScrollsUsed;
    }

    public UserData UserData { get; set; }
    public int PlayerStrength { get; set; }
    public int EnemyStrength { get; set; }
    public int DamageScrollsUsed { get; set; }
}