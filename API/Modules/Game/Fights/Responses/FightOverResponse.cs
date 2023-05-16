namespace API.Modules.Game.Fights.Responses;

public class FightOverResponse
{
    public FightOverResponse(string fightResult)
    {
        FightResult = fightResult;
    }

    public string FightResult { get; set; }
}